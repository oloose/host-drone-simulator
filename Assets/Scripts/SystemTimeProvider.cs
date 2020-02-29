using UnityEngine;
using System.Collections;
using System;

public class SystemTimeProvider : DateTimeProvider {

	public DateTime GetCurrent () {

		return DateTime.Now;
	}
}
