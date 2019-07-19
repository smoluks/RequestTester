﻿using System.Collections.Generic;
using System.Threading;

namespace RequestTester.Entities
{
    public class RequestCase
    {
        public Request request;

        public Dictionary<string, Response> Responses = new Dictionary<string, Response>();

        public CaseStatus _status = CaseStatus.NotDefined;

        public string Request { get => request.ToString(); }
        public string Status
        {
            get
            {
                switch (_status)
                {
                    case CaseStatus.Running:
                        return "Running";
                    case CaseStatus.Breaked:
                        return "Breaked";
                    case CaseStatus.Equals:
                        return "OK";
                    case CaseStatus.NotEquals:
                        return "Different";
                    case CaseStatus.Error:
                        return "Error";
                    default:
                        return "";
                }
            }
        }

        public RequestCase(Request request)
        {
            this.request = request;
        }


        public void CompareResults(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _status = CaseStatus.Breaked;
                return;
            }

            foreach (var responce in Responses.Values)
            {
                if (!responce.isSuccessed)
                {
                    _status = CaseStatus.Error;
                    return;
                }
            }

            string lastcontent = null;
            foreach (var responce in Responses.Values)
            {
                if (lastcontent != null)
                {
                    if (!responce.data.Equals(lastcontent))
                    {
                        _status = CaseStatus.NotEquals;
                        return;
                    }
                }
                lastcontent = responce.data;
            }
            _status = CaseStatus.Equals;
        }

        public enum CaseStatus
        {
            NotDefined,
            Running,
            Equals,
            NotEquals,
            Error,
            Breaked
        }
    }
}
