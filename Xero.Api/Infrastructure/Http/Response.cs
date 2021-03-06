﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Xero.Api.Infrastructure.Http
{
    internal class Response
    {
	    private string _body;

        internal Response(HttpWebResponse inner)
        {
            StatusCode = inner.StatusCode;
            ContentLength = (int) inner.ContentLength;
            ContentType = inner.ContentType;
			HttpWebRequest.DefaultMaximumErrorResponseLength = ContentLength;
			var stream = inner.GetResponseStream();
            if (stream != null)
            {
                Stream = new MemoryStream();
                stream.CopyTo(Stream);
                // rewind
                Stream.Seek(0, SeekOrigin.Begin);
            }

            Headers = new Dictionary<string, string>();

            foreach (var item in inner.Headers.AllKeys)
            {
                Headers[item] = inner.Headers[item];
            }

            inner.Close();
        }

        public string Body
        {
            get
            {
	            if (_body != null)
		            return _body;

				//Cache the body so it can be examined more than once when debugging
                using (var rdr = new StreamReader(Stream))
                {
                    var result = rdr.ReadToEnd();

                    Stream.Seek(0, SeekOrigin.Begin);
                        
                    _body = result;
                }

	            return _body;
            }
        }

        public Stream Stream { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public IDictionary<string,string> Headers { get; private set; }
    }
}