using System.Linq;
using System.Collections.Generic;
using DentalHub.Application.DTOs.Chat;
using DentalHub.Application.Interfaces;

namespace DentalHub.Application.Services
{
    public class ChatService : IChatService
    {
        // Bonus: Make questions configurable (dictionary)
        private static readonly Dictionary<string, string> _questions = new()
        {
            { "pain", "هل عندك وجع في السنان؟" },
            { "bleeding", "هل في نزيف؟" },
            { "swelling", "هل في تورم؟" },
            { "sensitivity", "هل في حساسية؟" }
        };

        private static readonly List<string> _flow = new()
        {
            "pain", "bleeding", "swelling", "sensitivity"
        };

        public ChatResponseDto ProcessNext(ChatRequestDto request)
        {
            var state = request.State;
            var answer = request.Answer;

            // If state is completely null or missing required fields, initialize
            if (state == null || string.IsNullOrWhiteSpace(state.CurrentQ))
            {
                return new ChatResponseDto
                {
                    Done = false,
                    Question = "عايز ايه؟ (تشخيص / تجميل / علاج)",
                    State = new ChatStateDto
                    {
                        Mode = null,
                        CurrentQ = "mode",
                        Answers = new Dictionary<string, string>(),
                        Asked = new List<string>()
                    }
                };
            }

            // Save the user's answer
            if (!string.IsNullOrWhiteSpace(answer))
            {
                state.Answers[state.CurrentQ] = answer;
                
                if (state.CurrentQ == "mode")
                {
                    state.Mode = answer;
                }

                if (!state.Asked.Contains(state.CurrentQ))
                {
                    state.Asked.Add(state.CurrentQ);
                }
            }

            // Determine next question based on flow
            // Mode is answered, now proceed with the questions
            string? nextQ = _flow.FirstOrDefault(q => !state.Asked.Contains(q));

            if (nextQ != null)
            {
                state.CurrentQ = nextQ;
                return new ChatResponseDto
                {
                    Done = false,
                    Question = _questions.GetValueOrDefault(nextQ) ?? "السؤال التالي؟",
                    State = state
                };
            }

            // If all questions answered
            return new ChatResponseDto
            {
                Done = true,
                Question = null,
                Result = new ChatResultDto
                {
                    Diagnosis = DetermineDiagnosis(state.Answers)
                },
                State = state
            };
        }

        // Bonus: Make diagnosis rules easily extendable
        private string DetermineDiagnosis(Dictionary<string, string> answers)
        {
            // Simple rule engine for diagnosis
            var pain = answers.GetValueOrDefault("pain");
            var bleeding = answers.GetValueOrDefault("bleeding");
            var swelling = answers.GetValueOrDefault("swelling");
            var sensitivity = answers.GetValueOrDefault("sensitivity");

            if (pain == "اه" && bleeding == "لا" && swelling == "لا" && sensitivity == "لا")
            {
                return "تسوس";
            }

            return "حالة أخرى - بحاجة لمزيد من الفحص";
        }
    }
}
