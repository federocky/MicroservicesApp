using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling Grpc Service {_configuration["GrpcPlatform"]}");

            //var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var handler = new HttpClientHandler
            {
                // ¡Solo para desarrollo! Acepta cualquier certificado
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"], new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });
            
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC server: {ex.Message}");
                return null;
            }
        }
    }
}