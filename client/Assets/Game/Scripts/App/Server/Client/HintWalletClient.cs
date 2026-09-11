using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class HintWalletClient : IHintWalletClient
    {
        private const string StatePath = "/api/v1/hints";
        private const string PurchasePath = "/api/v1/hints/purchase";
        private const string PurchaseActionKey = "PurchaseHint";

        private readonly IApiRequestExecutor _api;
        private readonly IIdempotentApiClient _idempotentApi;

        public HintWalletClient(IApiRequestExecutor api, IIdempotentApiClient idempotentApi)
        {
            _api = api;
            _idempotentApi = idempotentApi;
        }

        public UniTask<HintsStateResponse> GetState(CancellationToken cancellationToken = default)
        {
            return _api.GetAsync<HintsStateResponse>(StatePath, cancellationToken);
        }

        public UniTask<PurchaseHintResponse> Purchase(CancellationToken cancellationToken = default)
        {
            return _idempotentApi.PostIdempotentAsync<PurchaseHintRequest, PurchaseHintResponse>(
                PurchasePath,
                PurchaseActionKey,
                requestId => new PurchaseHintRequest { requestId = requestId.ToString() },
                cancellationToken);
        }
        
        [Serializable]                                                                                                                                                                                 
        private class PurchaseHintRequest : IIdempotentRequest                                                                                                                                         
        {                                                                                                                                                                                              
            public string requestId;                                                                                                                                                                     
                                                                                                                                                                                                         
            public Guid RequestId => Guid.Parse(requestId);                                                                                                                                                        
        }  
    }
}
