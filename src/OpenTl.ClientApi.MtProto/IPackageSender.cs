namespace OpenTl.ClientApi.MtProto
{
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface IPackageSender
    {
        Task<TResult> SendRequest<TResult>(IRequest<TResult> request);
    }
}