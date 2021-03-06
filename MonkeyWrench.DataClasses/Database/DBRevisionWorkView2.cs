﻿/*
 * DBRevisionWorkView2.cs
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

namespace MonkeyWrench.DataClasses
{
	partial class DBRevisionWorkView2
	{
		public DBState State
		{
			get { return (DBState) _state; }
			set { _state = (int) value; }
		}
	}
}
