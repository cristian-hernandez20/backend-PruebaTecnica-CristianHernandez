namespace Services {
    public interface IResultServices {

        ResultRandomDtos GenerateRandomResult();
        ResultDtos ProcessResults(UserBetDto userBet, ResultRandomDtos randomResult);

    }
    public class ResultServices() : IResultServices {

        public ResultRandomDtos GenerateRandomResult() {
            try {
                ResultRandomDtos responseResult = new();
                Random random = new();

                int numberRandom = random.Next(0, 36);
                responseResult.Number = numberRandom;

                string[] colors = ["red", "black"];
                responseResult.Color = colors[random.Next(colors.Length)];
                return responseResult;
            }
            catch (Exception) { throw; }
        }
        public ResultDtos ProcessResults(UserBetDto userBet, ResultRandomDtos randomResult) {
            try {
                ResultDtos responseResult = new() { BetAmount = userBet.BetAmount };

                decimal reward = 0m;

                //  Verificar si el usuario apostó al número y al color correcto
                if (userBet.Number == randomResult.Number && userBet.Color == randomResult.Color) { reward = userBet.BetAmount * 3; }

                // Verificar si el usuario apostó al color correcto
                else if (userBet.Color == randomResult.Color) { reward = userBet.BetAmount * 0.5m; }

                // Verificar si el usuario apostó a pares o impares del color correcto
                else if (userBet.IsEven && userBet.Color == randomResult.Color && ((userBet.IsEven && randomResult.Number % 2 == 0) || (!userBet.IsEven && randomResult.Number % 2 != 0))) {
                    reward = userBet.BetAmount;
                }

                responseResult.Reward = reward;

                return responseResult;
            }
            catch (Exception) { throw; }
        }
    }
}
