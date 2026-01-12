using System.Linq.Expressions;

namespace game_x.application.Contract.Jobs;

public interface IJobScheduler
{
    string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
}
