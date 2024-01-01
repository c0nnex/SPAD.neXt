using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IIsDeletable
    {
        void OnDelete();
    }
    public interface IIsEditable
    {
        /*
        void OnBeginEdit();
        void OnEndEdit();
        */
        bool CanEdit();
        bool CanDelete();
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

        public static ExpressionEvaluationResult CreateError(string msg) { return new ExpressionEvaluationResult() { ErrorMessage = msg }; }
        public static ExpressionEvaluationResult CreateResult(object result) { return new ExpressionEvaluationResult() { Result = result }; }
        public static readonly ExpressionEvaluationResult Empty = new ExpressionEvaluationResult();
    }


    public interface IDynamicExpression : IDisposable, IHasErrorInfo, IExternalExpression
    {
        string Name { get; }
        string Expression { get; }
        object Evaluate(ISPADEventArgs e = null);
        bool EvaluateBool(object value = null);
        void ExecuteWithValue(object value);
        bool Compile();

        event EventHandler<IMonitorableValue> VariableAdded;
        void EnableDebug(bool val);
        void RegisterFunction(string name, Func<object[], object> fun);
        void RegisterPrivateVariableFunction(Func<string, ExpressionEvaluationResult> fun);
        IDynamicExpression WithFunction(string name, Func<object[], object> fun);
        IDynamicExpression WithVariableFunction(Func<string, ExpressionEvaluationResult> fun);
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
