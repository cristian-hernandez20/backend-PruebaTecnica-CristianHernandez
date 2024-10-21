namespace Services {
    public interface IResultServices {

        ResponseResultDtos ProcessResults();
    }
    public class ResultServices() : IResultServices {

        public ResponseResultDtos ProcessResults() {
            try {
                ResponseResultDtos responseResult = new();
                int numberRandom = new Random().Next(0, 36);

                responseResult.Number = numberRandom;
                responseResult.Color = numberRandom % 2 == 0 ? "red" : "black";
                return responseResult;
            }
            catch (Exception) { throw; }
        }
    }
}
