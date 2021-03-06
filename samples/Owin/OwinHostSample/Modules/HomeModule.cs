﻿using Microsoft.Owin;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace OwinHostSample.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.Get[""] = ctx =>
                {
                    return View["Index"];
                };
            this.Post[""] = ctx =>
            {
                var gender = (string)this.Request.Form["gender"];
                var userAccountService = this.Context.ToOwinContext().GetUserAccountService();
                var account = userAccountService.GetByUsername(this.Context.CurrentUser.UserName);
                if (String.IsNullOrWhiteSpace(gender))
                {
                    account.RemoveClaim(ClaimTypes.Gender);
                }
                else
                {
                    // if you only want one of these claim types, uncomment the next line
                    //account.RemoveClaim(ClaimTypes.Gender);
                    account.AddClaim(ClaimTypes.Gender, gender);
                }
                userAccountService.Update(account);

                // since we've changed the claims, we need to re-issue the cookie that
                // contains the claims.
                var authSvc = this.Context.ToOwinContext().GetAuthenticationService();
                authSvc.SignIn(account);

                return this.Response.AsRedirect("~/");
            };
        }
    }
}
