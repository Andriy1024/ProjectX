﻿namespace ProjectX.Common.SeedWork
{
    public interface IOrderingOptions
    {
        string OrderBy { get; }

        bool Descending { get; }
    }
}
