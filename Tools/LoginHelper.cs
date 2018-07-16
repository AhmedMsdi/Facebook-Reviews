using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atreemo.Tools
{
    public class LoginHelper
    {
        private readonly HttpRequestBase _request;
        private readonly HttpResponseBase _response;
        private readonly HttpCookie _cookie;

        public LoginHelper(HttpRequestBase request, HttpResponseBase response)
        {
            _request = request;
            _response = response;
            _cookie = _request.Cookies["TSWA-Last-User"] ?? new HttpCookie("TSWA-Last-User")
            {
                Expires = DateTime.Now.AddMinutes(60)
            };
        }

        private int _authenticationAttempts;
        public int AuthenticationAttempts
        {
            get
            {
                if (_cookie != null &&
                !string.IsNullOrWhiteSpace(_cookie["AuthenticationAttempts"]))
                {
                    int.TryParse(_cookie["AuthenticationAttempts"], out _authenticationAttempts);
                }
                return _authenticationAttempts;
            }
            set
            {
                _authenticationAttempts = value;
                _cookie["AuthenticationAttempts"] = _authenticationAttempts.ToString();
                _cookie["CurrentUser"] = _currentUser;
                _cookie["PreviousUser"] = PreviousUser;
                _response.Cookies.Add(_cookie);
            }
        }

        private string _currentUser = string.Empty;
        public string CurrentUser
        {
            get
            {
                _currentUser = _request.LogonUserIdentity != null ?
                                     _request.LogonUserIdentity.Name : "";
                if (_cookie != null && !string.IsNullOrWhiteSpace(_cookie["CurrentUser"]))
                {
                    _currentUser = _cookie["CurrentUser"];
                }
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                _cookie["AuthenticationAttempts"] = _authenticationAttempts.ToString();
                _cookie["CurrentUser"] = _currentUser;
                _cookie["PreviousUser"] = PreviousUser;
                _response.Cookies.Add(_cookie);
            }
        }

        private string _previousUser = string.Empty;
        public string PreviousUser
        {
            get
            {
                if (_cookie != null && !string.IsNullOrWhiteSpace(_cookie["PreviousUser"]))
                {
                    _previousUser = _cookie["PreviousUser"];
                }
                return _previousUser;
            }
            set
            {
                _previousUser = value;
                _cookie["AuthenticationAttempts"] = _authenticationAttempts.ToString();
                _cookie["CurrentUser"] = _currentUser;
                _cookie["PreviousUser"] = _previousUser;
                _response.Cookies.Add(_cookie);
            }
        }

        /// <summary>
        /// Make sure the browser does not cache this page
        /// </summary>
        public void DisablePageCaching()
        {
            _response.Expires = 0;
            _response.Cache.SetNoStore();
            _response.AppendHeader("Pragma", "no-cache");
        }

        /// <summary>
        /// Send a 401 response
        /// <param name="returnUrl">
        /// For passing the returnUrl in order to force a refresh of the
        /// current page in case the cancel button in the Login popup has been clicked</param>
        /// </summary>
        public void Send401(string returnUrl)
        {
            _response.AppendHeader("Connection", "close");
            _response.StatusCode = 0x191;
            _response.Clear();
            _response.Write("Login cancelled. Please wait to be redirected...");
            // A Refresh header needs to be added in order to keep the application going after the
            // Windows Authentication Login popup is cancelled:
            _response.AddHeader("Refresh", "0; url=" + returnUrl);
            _response.End();
        }
    }
}