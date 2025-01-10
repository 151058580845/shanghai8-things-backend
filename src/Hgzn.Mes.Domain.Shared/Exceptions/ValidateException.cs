﻿namespace Hgzn.Mes.Domain.Shared.Exceptions
{
    public class ValidateException : CustomException
    {
        public ValidateException(string exceptionCode, params object?[] param) : base(exceptionCode)
        {
            ExceptionCode = string.Format(exceptionCode, param);
        }

        public override string ExceptionCode { get; protected set; }
    }
}