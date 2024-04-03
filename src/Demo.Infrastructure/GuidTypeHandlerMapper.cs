using System.Data;
using Dapper;
using System;

namespace Demo.Infrastructure;

internal sealed class GuidTypeHandlerMapper : SqlMapper.TypeHandler<Guid>
{ // https://stackoverflow.com/a/5920818
    public override void SetValue(IDbDataParameter parameter, Guid guid)
        => parameter.Value = guid.ToString();

    public override Guid Parse(object value)
    {
        // Dapper may pass a Guid instead of a string
        if (value is Guid guid)
        {
            return guid;
        }

        return new Guid((string)value);
    }
}
