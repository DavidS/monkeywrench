/*
 * DBWork.cs
 *
 * Authors:
 *   Rolf Bjarne Kvinge (RKvinge@novell.com)
 *   
 * Copyright 2009 Novell, Inc. (http://www.novell.com)
 *
 * See the LICENSE file included with the distribution for details.
 *
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;

namespace MonkeyWrench.DataClasses
{
	public partial class DBWork : DBRecord
	{
		public const string TableName = "Work";

		public DBWork ()
		{
		}

		public DBWork (IDataReader reader)
			: base (reader)
		{
		}

		public DBState State
		{
			get { return (DBState) state; }
			set { state = (int) value; }
		}

		public void CalculateSummary (TextReader reader)
		{
			string line;
			string id, file;
			char [] tr = new char [] { '(', ')', ' ' };
			List<string> failures = new List<string> ();
			int testsRun = 0, testFailures = 0, notRun = 0;
			float time = 0;
			// example line to parse: "Tests run: 8842, Failures: 14, Not run: 57, Time: 55.898 seconds"
			Regex testsRunPattern = new Regex("^Tests run: (?<testsRun>\\d+), Failures: (?<testFailures>\\d+), Not run: (?<notRun>\\d+), Time: (?<time>\\d+.?\\d*) seconds", RegexOptions.Compiled);

			try {
				while ((line = reader.ReadLine ()) != null) {
					if (line.StartsWith ("Failed:")) {
						line = line.Replace ("Failed:", "");
						int end = line.IndexOf ("--");
						if (end >= 0) {
							line = line.Substring (0, end).Trim ();
						} else {
							line = line.Trim ();
						}
						int space = line.IndexOf (' ');
						if (space > 0) {
							id = line.Substring (space).Trim (tr);
							line = line.Substring (0, space).Trim ();
							file = Path.GetFileName (line);
						} else {
							id = "-1";
							file = "<unknown>";
						}
						failures.Add (file + " " + id);
					}
					MatchCollection matches = testsRunPattern.Matches(line);
					if (matches.Count > 0) {
						Match match = matches[0];
						testsRun += Int32.Parse(match.Groups["testsRun"].Value);
						testFailures += Int32.Parse(match.Groups["testFailures"].Value);
						notRun += Int32.Parse(match.Groups["notRun"].Value);
						time += Single.Parse(match.Groups["time"].Value);
					}
				}

				if (testsRun > 0) {
					summary = string.Format ("Tests run: {0}, Failures: {1}, Not run: {2}, Time: {3:0.00} seconds", testsRun, testFailures, notRun, time);
				} else {
					summary = "-";
				}

				if (failures.Count > 0) {
					summary += " (Failures: ";
					for (int i = 0; i < failures.Count; i++) {
						summary += failures [i];
						if (i < failures.Count - 1)
							summary += ", ";
					}
					summary += ")";
				}
			} catch (Exception ex) {
				summary = ex.Message;
			}
		}
	}
}
