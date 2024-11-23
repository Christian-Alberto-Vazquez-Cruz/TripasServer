using System.IO;
using System.ServiceModel;

[ServiceContract]
public interface IGameService {
    // Operación para recibir trazos por streaming
    [OperationContract]
    void SendTrace(Stream traceStream);

    // Operación para marcar que el trazo está completo
    [OperationContract]
    void CompleteTrace(string player);
}
