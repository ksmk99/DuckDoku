using System;
using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.Presentation
{
    public class CurrencyPresenter : IInitializable, IDisposable
    {
        private readonly CurrencyView _view;
        private readonly ICurrencyService _currencyService;

        public CurrencyPresenter(CurrencyView view, ICurrencyService currencyService)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (currencyService == null)
            {
                throw new ArgumentNullException(nameof(currencyService));
            }

            _view = view;
            _currencyService = currencyService;
        }

        public void Initialize()
        {
            _currencyService.Changed += Render;

            Render();
        }

        public void Dispose()
        {
            _currencyService.Changed -= Render;
        }

        private void Render()
        {
            _view.SetValue(_currencyService.Balance);
        }
    }
}
