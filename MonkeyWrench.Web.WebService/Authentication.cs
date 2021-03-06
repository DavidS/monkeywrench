﻿/*
 * Authentication.cs
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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

using MonkeyWrench.Database;
using MonkeyWrench.DataClasses;
using MonkeyWrench.DataClasses.Logic;

namespace MonkeyWrench.WebServices
{
	public class Authentication {
		/// <summary>
		/// Authenticates the request with the provided user/pass.
		/// If no user/pass is provided, the method returns a response
		/// with no roles.
		/// If a wrong user/pass is provided, the method throws an exception.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="login"></param>
		/// <param name="response"></param>
		public static void Authenticate (HttpContext Context, DB db, WebServiceLogin login, WebServiceResponse response, bool @readonly)
		{
			Authenticate (Context.Request.UserHostAddress, db, login, response, @readonly);
		}

		public static void Authenticate (string user_host_address, DB db, WebServiceLogin login, WebServiceResponse response, bool @readonly)
		{
			string ip = user_host_address;
			int person_id;
			DBLoginView view = null;

			Logger.Log (2, "WebService.Authenticate (Ip4: {0}, UserHostAddress: {1}, User: {2}, Cookie: {3}, Password: {4}", login == null ? null : login.Ip4, user_host_address, login == null ? null : login.User, login == null ? null : login.Cookie, login == null ? null : login.Password);

			// Check if credentials were passed in
			if (login == null || string.IsNullOrEmpty (login.User) || (string.IsNullOrEmpty (login.Password) && string.IsNullOrEmpty (login.Cookie))) {
				Logger.Log (2, "No credentials.");
				return;
			}

			if (!string.IsNullOrEmpty (login.Ip4)) {
				ip = login.Ip4;
			} else {
				ip = user_host_address;
			}

			if (!string.IsNullOrEmpty (login.Password)) {
				DBLogin result = DBLogin_Extensions.Login (db, login.User, login.Password, ip, @readonly);
				if (result != null) {
					if (@readonly) {
						person_id = result.person_id;
					} else {
						view = DBLoginView_Extensions.VerifyLogin (db, login.User, result.cookie, ip);
						if (view == null) {
							Logger.Log (2, "Invalid cookie");
							return;
						}
						person_id = view.person_id;
					}
				} else {
					Logger.Log (2, "Invalid user/password");
					return;
				}
			} else {
				view = DBLoginView_Extensions.VerifyLogin (db, login.User, login.Cookie, ip);
				if (view == null) {
					Logger.Log (2, "Invalid cookie");
					return;
				}
				person_id = view.person_id;
				Logger.Log (2, "Verifying login, cookie: {0} user: {1} ip: {2}", login.Cookie, login.User, ip);
			}

			Logger.Log (2, "Valid credentials");

			DBPerson person = DBPerson_Extensions.Create (db, person_id);
			LoginResponse login_response = response as LoginResponse;
			if (login_response != null) {
				login_response.Cookie = view != null ? view.cookie : null;
				login_response.FullName = person.fullname;
				login_response.ID = person_id;
			}

			response.UserName = person.login;
			if (!string.IsNullOrEmpty (person.roles)) {
				response.UserRoles = person.roles.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				Logger.Log (2, "Roles are: {0}", string.Join (";", response.UserRoles));
			}
			Logger.Log (2, "Authenticate2 Roles are: {0}", response.UserRoles == null ? "null" : string.Join (";", response.UserRoles));
		}

		public static void VerifyUserInRole (HttpContext Context, DB db, WebServiceLogin login, string role, bool @readonly)
		{
			WebServiceResponse dummy = new WebServiceResponse ();
			Authenticate (Context, db, login, dummy, @readonly);

			if (!dummy.IsInRole (role)) {
				Logger.Log (2, "The user '{0}' has the roles '{1}', and requested role is: {2}", login.User, dummy.UserRoles == null ? "<null>" : string.Join (",", dummy.UserRoles), role);
				throw new HttpException (403, "You don't have the required permissions.");
			}
		}

		public static void VerifyUserInRole (string remote_ip, DB db, WebServiceLogin login, string role, bool @readonly)
		{
			WebServiceResponse dummy = new WebServiceResponse ();
			Authenticate (remote_ip, db, login, dummy, @readonly);

			if (!dummy.IsInRole (role)) {
				Logger.Log (2, "The user '{0}' has the roles '{1}', and requested role is: {2}", login.User, dummy.UserRoles == null ? "<null>" : string.Join (",", dummy.UserRoles), role);
				throw new HttpException (403, "You don't have the required permissions.");
			}
		}
	}
}