using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IHasErrorInfo
    {
        bool HasError { get; }
        string ErrorMessage { get; }
    }

    public interface IDynamicExpression : IDisposable, IHasErrorInfo, IExternalExpression
    {
        string Name { get; }
        object Evaluate();
        bool EvaluateBool();
        bool Compile();

        event EventHandler<IMonitorableValue> VariableAdded;
    }

    public interface IExternalExpression : IDisposable, IHasErrorInfo
    {
        IReadOnlyList<IMonitorableValue> Variables { get; }

        event EventHandler<IExternalExpression> ValueHasChanged;

        object GetValue();
    }

    public interface IDynamicNCalcExpression
    {
        object Evaluate(double val);
    }
}
