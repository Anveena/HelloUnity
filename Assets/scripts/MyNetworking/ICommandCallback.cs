using Photon.Client;

namespace MyNetworking
{
    public interface ICommandCallback
    {
        public void PrintAtLogPanel(string message, bool cleanHistory);

        public void OnEventData(MyCommand command, EventData eventData);

        public void OnOperationResponse(MyCommand command, OperationResponse operationResponse);

        public void OnOperationRequest(MyCommand command, OperationRequest operationRequest);

        public void OnMessage(MyCommand command,object message);
    }
}