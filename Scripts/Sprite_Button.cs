using System;
using BellyRub;

namespace CSharpAssembly
{
	public class Sprite_Button : ScriptController
	{
		public Sprite m_Button = Sprite.Invalid;
		public Sprite m_ButtonHovered = Sprite.Invalid;
		public Sprite m_ButtonPressed = Sprite.Invalid;

		private SpriteComponent m_SpriteComponent = null;

		private bool m_Pressed = false;
		private bool m_Hovered = false;

		public bool IsPressed() { return m_Pressed; }
		public bool IsHovered() { return m_Hovered; }

		void _SetSprite(Sprite sprite)
		{
			if (m_SpriteComponent == null)
				return;

			m_SpriteComponent.sprite = sprite;
		}

		void Start()
		{
			m_SpriteComponent = GetComponent<SpriteComponent>();
			if (m_SpriteComponent == null)
				Debug.LogWarning($"Button {entity.name} Doesn't have a Sprite Component!");
		}

		void Update()
		{
			if (m_Hovered && !m_Pressed)
				_SetSprite(m_ButtonHovered);

			else if (!m_Hovered && !m_Pressed)
				_SetSprite(m_Button);

			if (m_Pressed && !m_Hovered)
				m_Pressed = false;

			m_Hovered = false;
		}

		void OnClick(MouseCode mouse)
		{
			m_Pressed = true;
			_SetSprite(m_ButtonPressed);
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