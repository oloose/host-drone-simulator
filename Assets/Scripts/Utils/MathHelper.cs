//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.1026
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
namespace AssemblyCSharp.Utils
{
	public static class MathHelper
	{
		public static float NthRoot(float value, int n) 
		{
			// fix for negative values and odd exponents
			if (n % 2 == 1 && value < 0)
				return -(Mathf.Pow (-value, 1f / n));
			
			return Mathf.Pow (value, 1f / n);
		}
		
		public static float Cbrt(float value) 
		{
			
			return NthRoot (value, 3);
		}

		public static void Clamp(ref float value, float minimum, float maximum) {

			LimitMinimum (ref value, minimum);
			LimitMaximum (ref value, maximum);
		}
		
		public static void LimitMinimum(ref float value, float minimum) {
			
			if (value < minimum)
				value = minimum;
		}
		
		public static void LimitMaximum(ref float value, float maximum) {
			
			if (value > maximum)
				value = maximum;
		}

		public static float Interpolate(float minimum, float maximum, float ratio) {

			float value = maximum - minimum;
			value *= ratio;
			value += minimum;

			return value;
		}

		public static void Flatten(ref float value, float threshold){

			if (value > -threshold && value < threshold)
				value = 0;
		}

		public static float AngularDistance(float angle1, float angle2) {

			angle1 %= 360;
			angle2 %= 360;

			float result = angle2 - angle1;

			if (result < -180)
				return result + 360;

			if (result > 180)
				return result - 360;

			return result;
		}
		
		/**
		 * interpolates with an asymptotic function
		 */
		public static float Aserp (float value, float steepness, float max) {

			steepness = Mathf.Abs (steepness);

			if (value >= 0)
				return -max / (steepness * value + 1) + max;
			
			return -max / (steepness * value - 1) - max;
		}
		
		/**
		 * interpolates with an asymptotic function
		 */
		public static float Aserp (float value, float steepness, float max, float min) {

			float height = max + min;
			height /= 2;

			return Aserp (value, steepness, height) + max - height;
		}
	}
}
