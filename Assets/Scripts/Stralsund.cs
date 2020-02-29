//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.1026
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using TH = AssemblyCSharp.TimeHelper;

namespace AssemblyCSharp
{
	public static class Stralsund
	{
		// location
		public const float LATITUDE = 54.3392739f;
		public const float LONGITUDE = 13.0735586f;

		// summer sostice
		public const float EARLIEST_SUNRISE = 16380000;// = TH.TimeToMillies(4, 33);// = new TimeSpan(4, 33, 00);
		public const float LATEST_SUNSET = 78360000;// = TH.TimeToMillies(21, 46);// = new TimeSpan(21, 46, 00);
		public const float HIGHEST_ALTITUDE = 59.1f;

		// winter solstice sun time
		public const float LATEST_SUNRISE = 30480000;// = TH.TimeToMillies(8, 28);// = new TimeSpan(8, 28, 00);
		public const float EARLIEST_SUNSET = 56580000;// = TH.TimeToMillies(15, 43);// = new TimeSpan(15, 43, 00);
		public const float LOWEST_ALTITUDE = 12.22f;

//		static void Stralsund() {
//			
//			EARLIEST_SUNRISE = TH.TimeToMillies(4, 33);
//			LATEST_SUNSET = TH.TimeToMillies(21, 46);
//
//			LATEST_SUNRISE = TH.TimeToMillies(8, 28);
//			EARLIEST_SUNSET = TH.TimeToMillies(15, 43);
//		}
	}
}