using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IIsDeletable
    {
        void OnDelete();
    }


    public interface IHasErrorInfo
    {
        bool HasError { get; }
        string ErrorMessage { get; }
    }

    public sealed class ExpressionEvaluationResult
    {
        public bool HasError => !String.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage { get; set; } = null;
        public object Result { get; set; } = null;
        public bool HasResult => Result != null;
    }

    public interface IDynamicExpression : IDisposable, IHasErrorInfo, IExternalExpression
    {
        string Name { get; }
        string Expression { get; }
        object Evaluate(ISPADEventArgs e = null);
        bool EvaluateBool(object value = null);
        bool Compile();

        event EventHandler<IMonitorableValue> VariableAdded;
        void EnableDebug(bool val);
        void RegisterFunction(string name, Func<object[], object> fun);
        void RegisterPrivateVariableFunction(Action<string, ExpressionEvaluationResult> fun);
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
