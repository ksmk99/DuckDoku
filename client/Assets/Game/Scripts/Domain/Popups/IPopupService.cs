using Cysharp.Threading.Tasks;

namespace DuckDoku.Domain
{
    public interface IPopupService
    {
        UniTask Show(IPopupHandle popup);
    }
}
