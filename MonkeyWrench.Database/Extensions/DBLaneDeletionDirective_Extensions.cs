﻿/*
 * DBLaneDeletionDirective_Extensions.cs
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
using System.Linq;
using System.Text;

using MonkeyWrench.DataClasses;

namespace MonkeyWrench.Database
{
	public static class DBLaneDeletionDirective_Extensions
	{
		public static DBLaneDeletionDirective Create (DB db, int id)
		{
			return DBRecord_Extensions.Create (db, new DBLaneDeletionDirective (), DBLaneDeletionDirective.TableName, id);
		}
	}
}
