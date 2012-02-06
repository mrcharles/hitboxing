using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Diagnostics;

namespace HitBoxing
{
	class TerrainMap
	{
		class ValueContainer
		{
			int w,h;
			double[] values;

			public class Range
			{
				double min;
				double max;

				public Range(double _min, double _max)
				{
					min = _min;
					max = _max;
				}

				public double Normalize(double value)
				{
					return (value - min)  / (max - min);
				}
			};

			public ValueContainer(int width, int height)
			{
				w = width;
				h = height;


				values = new double[w * h];
			}
			public void Init(double v)
			{
				for (int i = 0; i < w * h; i++)
					values[i] = v;
			}

			public Range GetRange()
			{
				double min = 0.0, max=0.0;

				for (int i = 0; i < w * h; i++)
				{
					if (values[i] < min)
						min = values[i];

					if (values[i] > max)
						max = values[i];

				}

				return new Range(min, max);
			}

			Color ColorFromRange(Range range, double value)
			{ 
				//get our normalized lerp value
				if(value == 0.0f)
					return Color.AntiqueWhite;
				

				return Color.Lerp(Color.Black, Color.White, (float)range.Normalize(value));
			}

			public Texture2D Texture(GraphicsDevice device)
			{
				Color[] data = new Color[w * h];
				Range range = GetRange();

				for (int i = 0; i < w * h; i++)
				{
					data[i] = ColorFromRange(range, values[i]);
				}


				Texture2D tex = new Texture2D(device, w, h, false, SurfaceFormat.Color);
				tex.SetData(data);

				return tex;
			}

			public void sampleFrom(ValueContainer other, int x, int y)
			{
				setSample(x, y, other.sample(x, y));
			}
			
			/// <summary>
			/// pull a sample from the given x,y. indexes wrap.
			/// </summary>
			/// <param name="x">x</param>
			/// <param name="y">y</param>
			/// <returns>value</returns>
			public double sample(int x, int y)
			{
				return values[(x & (w - 1)) + (y & (h - 1)) * w];
			}

			public void sampleSquare(int x, int y, int size, double value)
			{
				int hs = size / 2;

				// a       b
				//
				//	   x
				//
				// c       d

				double a = sample(x - hs, y - hs);
				double b = sample(x + hs, y - hs);
				double c = sample(x - hs, y + hs);
				double d = sample(x + hs, y + hs);

				setSample(x, y, ((a + b + c + d) / 4.0) + value);

			}

			public void sampleDiamond(int x, int y, int size, double value)
			{
				int hs = size / 2;

				//    c
				//
				//a   x	  b
				//
				//    d

				double a = sample(x - hs, y);
				double b = sample(x + hs, y);
				double c = sample(x, y - hs);
				double d = sample(x, y + hs);

				setSample(x, y, ((a + b + c + d) / 4.0) + value);
			}

			/// <summary>
			/// set a sample at the given x, y. indexes wrap. 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <param name="value"></param>
			public void setSample(int x, int y, double value)
			{
				Debug.Assert(values[(x & (w - 1)) + (y & (h - 1)) * w] == 0.0);
				values[(x & (w - 1)) + (y & (h - 1)) * w] = value;
			}

			public ValueContainer Dupe()
			{
				ValueContainer ret = new ValueContainer(w, h);

				ret.values = (double[])values.Clone();

				return ret;
			}
		}

		ValueContainer init;
		List<ValueContainer> containers = new List<ValueContainer>();
		List<Texture2D> textures = new List<Texture2D>();
		int w, h, featuresize;
		int seed;
		Random random;

		bool IsPow2(int val)
		{
			int i = val;
			do{
				i = i >> 1;
				if( i == 0 )
					return true;
				if( val % i != 0 )
					return false;
			}while(true);
			
		}

		double frand()
		{
			return random.NextDouble() * 2.0 - 1.0;
		}
											 

		

		/// <summary>
		/// Create new terrain map.
		/// </summary>
		/// <param name="width">Width of the map in tiles. must be power of 2</param>
		/// <param name="height">Height of the map in tiles. must be power of 2</param>
		/// <param name="featuresize">Controls the density of features.</param>
		public TerrainMap(int width, int height, int _featuresize, int _seed = -1, double initialscale = 1.0, double magic = 0.0, double scalemod = 0.5, double scalemodmod = 1.0)
		{
			if (_seed == -1)
				seed = (int)DateTime.Now.Ticks;
			else 
				seed = _seed;
			featuresize = _featuresize;

			random = new Random(seed);

			w = width;
			h = height;
			if( !IsPow2(w) || !IsPow2(h) )
			{
				throw new Exception("Width and height must be power of 2");
			}

				

			init = new ValueContainer(w, h);
			init.Init(0.0);

			//initialize just our sparse points. The diamond-square algorithm will fill in the rest. 
			for( int y = 0; y < h; y += featuresize)
				for (int x = 0; x < w; x += featuresize)
				{
					init.setSample(x, y, frand());
				}

			int samplesize = featuresize;

			ValueContainer cur = init;
			double scale = initialscale;

			while (samplesize > 1)
			{
				//push our current map.
				containers.Add(cur);

				cur = DiamondSquare(cur, samplesize, scale);

				samplesize /= 2;
				scale *= scalemod + magic;
				scalemod *= scalemodmod;
			}
			containers.Add(cur);

		}

		ValueContainer DiamondSquare(ValueContainer map, int stepsize, double scale)
		{
			ValueContainer ret = map.Dupe();

			int halfstep = stepsize / 2;

			for (int y = halfstep; y < h + halfstep; y += stepsize)
			{
				for (int x = halfstep; x < w + halfstep; x += stepsize)
				{
					ret.sampleSquare(x, y, stepsize, frand() * scale);
				}
			}

			for (int y = 0; y < h; y += stepsize)
			{
				for (int x = 0; x < w; x += stepsize)
				{
					ret.sampleDiamond(x + halfstep, y, stepsize, frand() * scale);
					ret.sampleDiamond(x, y + halfstep, stepsize, frand() * scale);
				}
			}

			return ret;

		}

		public void CreateTextures(GraphicsDevice device)
		{
			foreach( ValueContainer c in containers)
			{
				textures.Add(c.Texture(device));
			}

		}

		public int GetMaxIndex()
		{
			return textures.Count - 1;
		}

		public Texture2D GetTextureFromIndex( int i )
		{
			return textures.ElementAt(i % textures.Count);

		}

	}
}
