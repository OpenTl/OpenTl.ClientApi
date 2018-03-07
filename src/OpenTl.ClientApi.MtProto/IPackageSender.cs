namespace OpenTl.ClientApi.MtProto
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface IPackageSender
    {
        Task<TResult> SendRequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken);
    }
}