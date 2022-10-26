// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class InternalServerErrorErrorResponse : ErrorResponse
    {
        private static string _env = null;
        public InternalServerErrorErrorResponse()
            : base(500, HttpStatusCode.InternalServerError.ToString())
        {
        }

        public InternalServerErrorErrorResponse(string message)
            : base(500, HttpStatusCode.InternalServerError.ToString(), ProcessMessage(message))
        {
            AddInfo();
        }

        private void AddInfo()
        {
            if (IsProductionEnvironment())
            {
                this.Notifications = new List<Notification>(this.Notifications)
                {
                    Notification.Info("If this error keeps occuring, contact support.")
                }.ToArray();
            }
        }

        public InternalServerErrorErrorResponse(Notification[] notifications)
            : base(500, HttpStatusCode.InternalServerError.ToString(), notifications)
        {
            AddInfo();
        }

        public static bool IsNonProductionEnvironment()
        {
            var env = _env ?? (_env = (Environment.GetEnvironmentVariable("Environment") ?? Environment.GetEnvironmentVariable("Env")) ?? string.Empty);
            return env == string.Empty ||
                env.Equals("Dev", StringComparison.OrdinalIgnoreCase) ||
                env.Equals("Test", StringComparison.OrdinalIgnoreCase) ||
                env.Equals("Acc", StringComparison.OrdinalIgnoreCase) ||
                env.Equals("Staging", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsProductionEnvironment() => !IsNonProductionEnvironment();

        private static string ProcessMessage(string message)
        {
            if(IsProductionEnvironment()){
                message = "An unexpected error has occured, please try again later.";
            }
            return message;
        }
    }
}