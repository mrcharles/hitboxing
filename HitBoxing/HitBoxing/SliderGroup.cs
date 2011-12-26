using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using System.Diagnostics;

namespace HitBoxing
{
	class SliderGroup
	{
		public delegate void ValueChangedDelegate(object state);
		
		public interface ISlider
		{
			float GetPercentage();
			void Increment();
			void Decrement();
		}
		public class SliderState<T>
		{
			public T Step;
			public T Min;
			public T Max;
			T _Value;
			public ValueChangedDelegate OnChanged;

			public T Value
			{
				get
				{
					return _Value;
				}
				set
				{
					_Value = value;
					OnChanged(this);
				}

			}

			public SliderState(T step, T min, T max, T value )
			{
				Step = step;
				Min = min;
				Max = max;
				_Value = value;
			}
		}
		public class DoubleSlider : ISlider
		{
			SliderState<double> state;

			public DoubleSlider(double step, double min, double max, double value, ValueChangedDelegate changedDelegate = null)
			{
				state = new SliderState<double>(step, min, max, value);
				state.OnChanged = changedDelegate;
			}

			float ISlider.GetPercentage()
			{
				return (float)((state.Value - state.Min) / (state.Max - state.Min));
			}

			void ISlider.Increment()
			{
				state.Value = Math.Min(state.Value + state.Step, state.Max); 
			}

			void ISlider.Decrement()
			{
				state.Value = Math.Max(state.Value - state.Step, state.Min);
			}
		}

		public class IntSlider : ISlider
		{
			SliderState<int> state;

			public IntSlider(int step, int min, int max, int value, ValueChangedDelegate changedDelegate = null)
			{
				state = new SliderState<int>(step, min, max, value);
				state.OnChanged = changedDelegate;
			}

			float ISlider.GetPercentage()
			{
				return ((float)(state.Value - state.Min) / (float)(state.Max - state.Min));
			}

			void ISlider.Increment()
			{
				state.Value = Math.Min(state.Value + state.Step, state.Max);
			}

			void ISlider.Decrement()
			{
				state.Value = Math.Max(state.Value - state.Step, state.Min);
			}
		}

		class Widget
		{
			public ISlider value;
			public Vector2 pos;
			public bool selected;

			public Widget(Vector2 _pos, ISlider _value)
			{
				pos = _pos;
				value = _value;
			}
		}

		Texture2D texture;
		List<Widget> widgets = new List<Widget>();
		Widget currentselection;
		int selected;
		Vector2 position;

		int WidgetSpacing = 40;
		int WidgetHeight = 20;
		int WidgetWidth = 200;

		public SliderGroup(Texture2D tex, Vector2 pos )
		{
			texture = tex;
			position = pos;

		}

		public void SelectSlider(int index)
		{
			Debug.Assert(index >= 0 && index < widgets.Count);
			selected = index;
			if (currentselection != null)
				currentselection.selected = false;
			currentselection = widgets.ElementAt(index);

			currentselection.selected = true;
		}

		public void SelectNone()
		{
			currentselection = null;
		}

		public void SelectNext()
		{
			selected++;

			if (selected >= widgets.Count)
				selected = 0;
			SelectSlider(selected);
		}

		public void SelectPrev()
		{
			selected--;

			if (selected < 0)
				selected = widgets.Count - 1;
			SelectSlider(selected);
		
		}

		public void AdjustRight()
		{
			Debug.Assert(currentselection != null);
			currentselection.value.Increment();
		}

		public void AdjustLeft()
		{
			Debug.Assert(currentselection != null);
			currentselection.value.Decrement();
		}

		Vector2 NewWidgetPosition()
		{
			return position + new Vector2(position.X, position.Y + WidgetHeight * widgets.Count);
		}

		Rectangle BarRectFromPosition(Vector2 pos)
		{
			return new Rectangle((int)pos.X, (int)pos.Y, WidgetWidth, WidgetHeight / 2);
		}

		Rectangle sliderRectFromBarRect(Rectangle bar, float pct)
		{
			int x = (int)(bar.X + (pct * bar.Width));
			int y = (int)(bar.Y - bar.Height / 2.0f);

			int width = WidgetWidth / 10;
			int height = WidgetHeight;

			x -= width / 2;

			return new Rectangle(x, y, width, height);
		}

		public void RegisterValue(ISlider value)
		{
			Widget w = new Widget(NewWidgetPosition(), value);

			widgets.Add(w);
		}

		public void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			foreach (Widget w in widgets)
			{
				float pct = w.value.GetPercentage();
				//draw bar
				Rectangle r = BarRectFromPosition(w.pos);
				batch.Draw(texture, r, Color.White);

				//draw slider
				Rectangle s = sliderRectFromBarRect(r, pct);
				batch.Draw(texture, s, w.selected ? Color.Red : Color.White);
			}
			batch.End();
		}
	}
}
