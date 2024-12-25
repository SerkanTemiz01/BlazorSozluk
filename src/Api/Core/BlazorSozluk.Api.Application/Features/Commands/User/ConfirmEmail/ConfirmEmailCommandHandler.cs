using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.User.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailConfirmationRepository emailConfirmationRepository;

        public ConfirmEmailCommandHandler(IUserRepository userRepository, IEmailConfirmationRepository emailConfirmationRepository)
        {
            this.userRepository = userRepository;
            this.emailConfirmationRepository = emailConfirmationRepository;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var confirmation = await emailConfirmationRepository.GetByIdAsync(request.ConfirmationId);
            if (confirmation == null) 
                throw new DatabaseValidationException("Confirmation not found!");

            var dbUser = await userRepository.GetSingleAsync(x => x.EmailAdress == confirmation.NewEmailAdress);
            if (dbUser == null)
                throw new DatabaseValidationException("User not found with this email");
            if (dbUser.EmailConfirmed)
                throw new DatabaseValidationException("Email already confirmed!");

            dbUser.EmailConfirmed = true;
            await userRepository.UpdateAsync(dbUser);
            return true;
        }
    }
}
