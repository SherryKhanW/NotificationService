using Elastic.Clients.Elasticsearch;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Repositories;

public class EmailLetterRepository : GenericRepository<EmailLetter>
{
    public EmailLetterRepository(
        AppDbContext context,
        ElasticsearchClient elasticsearch)
        : base(context, elasticsearch)
    {
    }
}