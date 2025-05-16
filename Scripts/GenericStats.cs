using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using BellyRub;

namespace CSharpAssembly
{
	public class StatGrowthToolData
	{
		public int startLevel = 1;
		public List<int> statGrowthFromLastLevel = new List<int>();
		public List<int> statTotalAtLevel = new List<int>();
	}


	public class SubStat
	{

		//DEFAULTS ARE HP FROM RING GAME.

		public string name = "";
		public float currentMax = 1.0f;


		public int startMax = 300;

		public int statSoftCap1 = 25;
		public float influenceCurve1 = 1.5f;
		public int softCap1Adds = 500;


		public int statSoftCap2 = 40;
		public float influenceCurve2 = 1.1f;
		public int softCap2Adds = 650;

		public int statSoftCap3 = 60;
		public float influenceCurve3 = 1.2f;
		public int softCap3Adds = 450;

		public int statHardCap = 99;
		public float influenceCurve4 = 1.2f;
		public int hardCapAdds = 200;

		public int postHardCapAddPerLevel = 1;



	}

	public class StatInfluencePair
	{
		public string mainStatName = "";
		public string subStatName = "";

		public float influence = 0;
	}

	public class MainStat
	{
		//defaults match vigor of ring as an easy example.
		public string name = "";
		public int statLevel = 1;

		public int maxLevelOfMainStat = 99;

	}

	public class GenericStats : ScriptController
	{

		public void SetLevelOfMainStat(string statToSet, int newLevel)
		{
			for (int i = 0; i<mainStats.Count; i++)
			{
				if (mainStats[i].name == statToSet)
				{
					mainStats[i].statLevel = newLevel;
				}
			}
		}

		public int GetMainStatLevel(string mainStatToGet)
		{
			int mainStatLevel = 1;

			for (int i = 0; i<mainStats.Count; i++)
			{
				if (mainStats[i].name == mainStatToGet)
				{
					mainStatLevel = mainStats[i].statLevel;
				}
			}
			return mainStatLevel;
		}

		public List<StatInfluencePair> GetInfluence(string subStatNameToLookForInfluences)
		{

			List<StatInfluencePair> relevantInfluencePairs = new List<StatInfluencePair>();

			for (int i = 0; i < influencePairs.Count; i++)
			{
				if (influencePairs[i].subStatName == subStatNameToLookForInfluences)
				{
					StatInfluencePair relevantInf = new StatInfluencePair()
					{
						subStatName = influencePairs[i].subStatName,
						mainStatName = influencePairs[i].mainStatName,
						influence = influencePairs[i].influence
					};
					relevantInfluencePairs.Add(relevantInf);
				}
			}

			return relevantInfluencePairs;
		}



		public List<MainStat> mainStats = new List<MainStat>();

		public List<SubStat> SubStats = new List<SubStat>();

		public List<StatInfluencePair> influencePairs = new List<StatInfluencePair>();


		void initialiseTestStats()
		{
			//example is hp against vigor from ring game.

			//string nameToSet, int statLevelToSet, int statSoftCap1ToSet, int statSoftCap2ToSet, int statSoftCap3ToSet, int statHardCapToSet
			initialiseMainStat("vigor",1);

			//Base name and start, then soft cap levels, then influence, then the amount that should be gained by each softcap.
			
			initialiseSubStat("hp",300,
								25,40,60,99,
								1.5f,1.1f,1.2f,1.2f,
								500,650,450,200
			);			

			StatInfluencePair testInfluence = new StatInfluencePair();
			testInfluence.influence = 1;
			testInfluence.subStatName = "hp";
			testInfluence.mainStatName = "vigor";

			influencePairs.Add(testInfluence);


		}



		void initialiseMainStat(string nameToSet, int statLevelToSet)
		{
			MainStat statToMake = new MainStat();
			statToMake.name = nameToSet;
			statToMake.statLevel = statLevelToSet;


			mainStats.Add(statToMake);
		}



		void initialiseSubStat(string nameToSet,int startMaxToSet, 
		int softCap1ToSet,int softCap2ToSet,int softCap3ToSet, int hardCapToSet, 
		float influenceCurve1ToSet, float influenceCurve2ToSet,  float influenceCurve3ToSet, float influenceCurve4ToSet,
		int softCap1AddsToSet, int softCap2AddsToSet,int softCap3AddsToSet,  int hardCapAddsToSet)
		{
			SubStat calculatedStatToMake = new SubStat() 
			{};
			//defaults are for hp. easiest to add.

			calculatedStatToMake.name = nameToSet; 
			
			calculatedStatToMake.statSoftCap1 = softCap1ToSet; 
			calculatedStatToMake.statSoftCap2 = softCap2ToSet;
			calculatedStatToMake.statSoftCap3 = softCap3ToSet;
			calculatedStatToMake.statHardCap = hardCapToSet;
			calculatedStatToMake.startMax = startMaxToSet; 

			calculatedStatToMake.influenceCurve1 = influenceCurve1ToSet;
			calculatedStatToMake.softCap1Adds = softCap1AddsToSet;
			calculatedStatToMake.influenceCurve2 = influenceCurve2ToSet;
			calculatedStatToMake.softCap2Adds = softCap2AddsToSet;
			calculatedStatToMake.influenceCurve3 = influenceCurve3ToSet;
			calculatedStatToMake.softCap3Adds = softCap3AddsToSet;
			calculatedStatToMake.influenceCurve4 = influenceCurve4ToSet;
			calculatedStatToMake.hardCapAdds = hardCapAddsToSet;


			SubStats.Add(calculatedStatToMake);

		}

		SubStat GetSubStat(string nameToGet)
		{
			SubStat subStatCollected = new SubStat();

			for (int i = 0; i < SubStats.Count; i++)
			{
				if (SubStats[i].name == nameToGet)
				{
					subStatCollected = SubStats[i];
				}

			}
			return subStatCollected;
		}

		int GetSubStatValue(SubStat calculatedStat)
		{

			List<StatInfluencePair> listOfRelevantPairs = GetInfluence(calculatedStat.name);

			//stat is an homogenised level based on the levels of the main stat.
			int homogenisedStatLevel = 1;

			for (int i = 0; i < listOfRelevantPairs.Count; i++)
			{
				StatInfluencePair pair = listOfRelevantPairs[i];

				for (int j = 0; j < mainStats.Count; j++)
				{
					if (mainStats[j].name == pair.mainStatName)
					{
						//stats[j].statLevel is the main stat level, and then we are modifying by influence.
						//We want to also add modifiers here, in the sense of things like "Add 5% influence of vigor on hp" on a piece of armor.
						//so we really want something like:
						/*

							int statLevelToMod = stats[j].statLevel
							foreach(modification in relevantModifications)
							{
								if (relevantModifications[i].mainStatName == stats[j].name)
								{	
									switch modType{
										modType: influenceIncreaseByPercent
											statLevelToMod = statLevelToMod * (1 + relevantModifications[i].percent);
											break;
										modType: levelIncreasePer10Levs
											statLevelToMod = statLevelToMod + (relevantModifications[i].levelIncreasePer10Levs * (statLevelToMod/10));
											break;
									}
									
								}

							}


							homogenisedStatLevel += (int)(stats[j].statLevel * listOfRelevantPairs[i].influence);

							Or Some shi like this anyways. Its possible we may want more rules, penalties, types of modifications, ect. This culd be a big section as a lot of different factors may modify at this point before global modifications.

							then we may want a later modification to the derived stat final score after doing this modification, but thats in a later section.(example marked there.)	

						*/
						homogenisedStatLevel += (int)(mainStats[j].statLevel * pair.influence);
					}
				}
			}


			//initial base value.
			calculatedStat.currentMax = calculatedStat.startMax;
			int h = homogenisedStatLevel;
			if (h > 1)
			{
				float j = Math.Min(h,calculatedStat.statSoftCap1);
				calculatedStat.currentMax += calculatedStat.softCap1Adds * (float)Math.Pow(((j-1) / (calculatedStat.statSoftCap1-1)), (float)calculatedStat.influenceCurve1);
			}

			if (h > calculatedStat.statSoftCap1)
			{
				float j = Math.Min(h,calculatedStat.statSoftCap2);
				calculatedStat.currentMax += calculatedStat.softCap2Adds * (float)Math.Pow(((j - calculatedStat.statSoftCap1) / (calculatedStat.statSoftCap2-calculatedStat.statSoftCap1)), (float)calculatedStat.influenceCurve2);
			}
			
			if (h > calculatedStat.statSoftCap2)
			{
				float j = Math.Min(h,calculatedStat.statSoftCap3);
				calculatedStat.currentMax += calculatedStat.softCap3Adds * (float)(1 - Math.Pow((1-((j - calculatedStat.statSoftCap2)/(calculatedStat.statSoftCap3-calculatedStat.statSoftCap2))), (float)calculatedStat.influenceCurve3));
			}
		
			if (h > calculatedStat.statSoftCap3)
			{
				float j = Math.Min(h,calculatedStat.statHardCap);
				calculatedStat.currentMax += calculatedStat.hardCapAdds * (float)(1 - Math.Pow((1-((j - calculatedStat.statSoftCap3)/(calculatedStat.statHardCap-calculatedStat.statSoftCap3))), (float)calculatedStat.influenceCurve4));
			}
			
			if (h > calculatedStat.statHardCap)
			{
				float j = Math.Min(h,200);
				//Work out if we want additional for hardcap overrides. (probably tbh. This should take into account modifiers.)
				//For the moment, add 1 for each level? This is the derived stat going past the hard cap of the derived level. Possible if we want to modify, ie: hp from vigor +5% on an equip, might take us over the hard cap.
				calculatedStat.currentMax += (j-calculatedStat.statHardCap)* calculatedStat.postHardCapAddPerLevel;
			}



			//Global modifications here. These will not be effected by influence modifications as we are applying them later, like example below.
			/*

				int statLevelToMod = stats[j].statLevel
				foreach(modification in relevantModifications)
				{
					if (relevantModifications[i].derived.name == derivedStats[j].name)
					{	
						switch modType{
							modType: staticAddition
								statLevelToMod = statLevelToMod * (1 + relevantModifications[i].percent);
								break;
						modType: levelIncreasePer10Levs
								statLevelToMod = statLevelToMod + (relevantModifications[i].levelIncreasePer10Levs * (statLevelToMod/10));
								break;
						}
									
					}

				}


				homogenisedStatLevel += (int)(stats[j].statLevel * listOfRelevantPairs[i].influence);

			*/
			

			return (int)calculatedStat.currentMax;

		}


		public StatGrowthToolData GetStatGrowthToolData(int levelRangeMin = 1, int levelRangeMax = 200, string subStatName = "", string specificRelationshipWithMainStatName = "")
		{
			
			StatGrowthToolData dataToReturn = new StatGrowthToolData();
			dataToReturn.startLevel = levelRangeMin;

			if(subStatName == "")
			{
				dataToReturn.statGrowthFromLastLevel.Add(1);
				dataToReturn.statTotalAtLevel.Add(1);
				return dataToReturn;

			}

			if(levelRangeMin < 1 || levelRangeMin >= levelRangeMax)
			{
				levelRangeMin = 1;
			}

			if(levelRangeMax < 1 || levelRangeMax >= 200)
			{
				levelRangeMax = 200;
			}



			List<StatInfluencePair> listOfRelevantPairs = GetInfluence(subStatName);
			SubStat calculatedStat = GetSubStat(subStatName);

			//specificRelationshipWithMainStatName is blank if we only want total homogenised level. If we add a specific relationship, that can be done later.
			if(specificRelationshipWithMainStatName == "")
			{	
				int lastValue = 0;
				int growthFromLast = 0;
 
				for (int i = 0; i < levelRangeMax; i++)
				{
					calculatedStat.currentMax = calculatedStat.startMax;
		
					if (i > 1)
					{
						float j = Math.Min(i,calculatedStat.statSoftCap1);
						calculatedStat.currentMax += calculatedStat.softCap1Adds * (float)Math.Pow(((j-1) / (calculatedStat.statSoftCap1-1)), (float)calculatedStat.influenceCurve1);
					}

					if (i > calculatedStat.statSoftCap1)
					{
						float j = Math.Min(i,calculatedStat.statSoftCap2);
						calculatedStat.currentMax += calculatedStat.softCap2Adds * (float)Math.Pow(((j - calculatedStat.statSoftCap1) / (calculatedStat.statSoftCap2-calculatedStat.statSoftCap1)), (float)calculatedStat.influenceCurve2);
					}
			
					if (i > calculatedStat.statSoftCap2)
					{
						float j = Math.Min(i,calculatedStat.statSoftCap3);
						calculatedStat.currentMax += calculatedStat.softCap3Adds * (float)(1 - Math.Pow((1-((j - calculatedStat.statSoftCap2)/(calculatedStat.statSoftCap3-calculatedStat.statSoftCap2))), (float)calculatedStat.influenceCurve3));
					}
			
					if (i > calculatedStat.statSoftCap3)
					{
						float j = Math.Min(i,calculatedStat.statHardCap);
						calculatedStat.currentMax += calculatedStat.hardCapAdds * (float)(1 - Math.Pow((1-((j - calculatedStat.statSoftCap3)/(calculatedStat.statHardCap-calculatedStat.statSoftCap3))), (float)calculatedStat.influenceCurve4));
					}
			
					if (i > calculatedStat.statHardCap)
					{
						float j = Math.Min(i,200);
						//Work out if we want additional for hardcap overrides. (probably tbh. This should take into account modifiers.)
						//For the moment, add 1 for each level? This is the derived stat going past the hard cap of the derived level. Possible if we want to modify, ie: hp from vigor +5% on an equip, might take us over the hard cap.
						calculatedStat.currentMax += (j-calculatedStat.statHardCap)* calculatedStat.postHardCapAddPerLevel;
					}

					growthFromLast = (int)calculatedStat.currentMax - lastValue;
					

					lastValue = (int)calculatedStat.currentMax;

					dataToReturn.statGrowthFromLastLevel.Add(growthFromLast);
					dataToReturn.statTotalAtLevel.Add(lastValue);

				}
			}
			return dataToReturn;
		}


		public void GraphStatData(StatGrowthToolData dataToGraph)
		{

			GetStatGrowthToolData();
		}


		void Start()
		{
			initialiseTestStats();
			StatGrowthToolData dataToGraph = GetStatGrowthToolData(1,200,"hp","");

			for(int lev = 1; lev < 200; lev++)
			{	
				Debug.Log("Level " + lev + " Adds " + dataToGraph.statGrowthFromLastLevel[lev] + " stat and the total is currently " + dataToGraph.statTotalAtLevel[lev]);
			}

		}

		void Update()
		{

		}
	}
}