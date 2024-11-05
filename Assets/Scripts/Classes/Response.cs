using System;
using System.Collections.Generic;

public class Response<T> {
    public ResponseStatus status;
    public Message message;
    public T data;
}

public delegate void ResponseAction<T>(Response<T> response);

public enum ResponseStatus
{
    SUCCESS, FAIL
}