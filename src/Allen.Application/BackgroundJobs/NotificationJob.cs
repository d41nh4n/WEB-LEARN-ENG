using Quartz;

namespace Allen.Application;

public class NotificationJob : IJob
{
	private readonly IPushSubscriptionService pushSubscriptionService;
	private readonly IFlashCardsService flashCardsService;
	private readonly IEmailService emailService;

	public NotificationJob(IPushSubscriptionService _pushSubscriptionService, IFlashCardsService _flashCardsService, IEmailService _emailService)
	{
		pushSubscriptionService = _pushSubscriptionService;
		flashCardsService = _flashCardsService;
		emailService = _emailService;
	}
	public async Task Execute(IJobExecutionContext context)
	{
		var emailContent = new EmailContent
		{
			To = "ddhung2003@gmail.com",
			Subject = "Study Reminder",
			Body = "message"
		};
		await emailService.SendMailAsync(emailContent);

		await pushSubscriptionService.SendNotificationToStudyFlashCardTodayAsync();
	}
}
