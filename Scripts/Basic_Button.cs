using System;
using BellyRub;

namespace CSharpAssembly
{
	public class Basic_Button : ScriptController
	{
		public Colour4 m_BgColour = Colour4.DarkGrey;
		public Colour4 m_HoveredColour = Colour4.Grey;
		public Colour4 m_PressedColour = Colour4.LightGrey;

		private bool m_Pressed = false;
		private bool m_Hovered = false;

		public bool IsPressed() { return m_Pressed; }
		public bool IsHovered() { return m_Hovered; }

		void Start()
		{

		}

		void Update()
		{
			if (m_Hovered && !m_Pressed)
				entity.material.tint = m_HoveredColour;

			else if (!m_Hovered && !m_Pressed)
				entity.material.tint = m_BgColour;

			if (m_Pressed && !m_Hovered)
				m_Pressed = false;

			m_Hovered = false;
		}

		void OnClick(MouseCode mouse)
		{
			m_Pressed = true;
			entity.material.tint = m_PressedColour;
		}

		void OnHovered()
		{
			m_Hovered = true;
		}

		void OnReleased(MouseCode mouse)
		{
			m_Pressed = false;
		}
	}
}