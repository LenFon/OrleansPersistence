using Orleans;
using System;
using System.Threading.Tasks;

namespace IGrains
{
    public interface IHello: IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
