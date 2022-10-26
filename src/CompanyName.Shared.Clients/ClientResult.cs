// Copyright (c) Mastronardi Software (mastrosoft.com). All Rights Reserved.
// Licensed under Mozilla Public License, Version 2.0

using CompanyName.Shared.Common;
using System;
using System.Net;

namespace CompanyName.Shared.Clients
{
    public class ClientResult{
        public ClientResult()
        {

        }
        public HttpStatusCode StatusCode { get; set; }
        public ClientResult(HttpStatusCode statusCode, Notification[] notifications) : this(notifications)
        {
            this.StatusCode = statusCode;
        }
        public ClientResult(Notification[] notifications)
        {
            this.Notifications = notifications;
        }
        public Notification[] Notifications { get; protected set; }
    }
    public class ClientResult<T> : ClientResult
    {

        public T Result { get; }

        public bool IsSuccessful(bool ignoreWarnings) => ((int)StatusCode >= 200 && (int)StatusCode <= 299) && Notifications.IsSuccessful(ignoreWarnings);

        [Obsolete("Use Clientresult with StatusCode instead")]
        public ClientResult(T result, Notification[] notifications) : base(notifications)
        {
            Result = result;
        }
        public ClientResult(HttpStatusCode statusCode, T result, Notification[] notifications) : base(statusCode, notifications)
        {
            //StatusCode = statusCode;
            Result = result;
        }
        //[Obsolete("Use ClientResult constructor with StatusCode instead")]
        public ClientResult(T result) : this(HttpStatusCode.OK, result, new Notification[0])
        {
        }
        public ClientResult(HttpStatusCode statusCode, T result) : this(statusCode, result, new Notification[0])
        {
        }
        [Obsolete("Use ClientResult constructor with StatusCode instead")]
        public ClientResult(Notification[] notifications) : this(default(T), notifications)
        {
        }
        public ClientResult(HttpStatusCode statusCode, Notification[] notifications) : this(statusCode, default(T), notifications)
        {
        }
    }
}