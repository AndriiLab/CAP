using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;

namespace DotNetCore.CAP.AmazonSQS
{
    public static class AmazonPolicyExtensions
    {
        public static bool HasSqsPermission(this Policy policy, string topicArn, string sqsQueueArn)
        {
            foreach (var statement in policy.Statements)
            {
                var containsResource = statement.Resources.Any(r => r.Id.Equals(sqsQueueArn));

                if (!containsResource)
                {
                    continue;
                }

                foreach (var condition in statement.Conditions)
                {
                    if ((string.Equals(condition.Type, ConditionFactory.StringComparisonType.StringLike.ToString(), StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(condition.Type, ConditionFactory.StringComparisonType.StringEquals.ToString(), StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(condition.Type, ConditionFactory.ArnComparisonType.ArnEquals.ToString(), StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(condition.Type, ConditionFactory.ArnComparisonType.ArnLike.ToString(), StringComparison.OrdinalIgnoreCase)) &&
                        string.Equals(condition.ConditionKey, ConditionFactory.SOURCE_ARN_CONDITION_KEY, StringComparison.OrdinalIgnoreCase) &&
                        condition.Values.Contains(topicArn))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void AddSqsPermissions(this Policy policy, IEnumerable<string> topicArns, string sqsQueueArn)
        {
            var statement = new Statement(Statement.StatementEffect.Allow);
            statement.Actions.Add(SQSActionIdentifiers.SendMessage);
            statement.Resources.Add(new Resource(sqsQueueArn));
            statement.Principals.Add(new Principal("*"));
            foreach (var topicArn in topicArns)
            {
                statement.Conditions.Add(ConditionFactory.NewSourceArnCondition(topicArn));
            }

            policy.Statements.Add(statement);
        }

        public static void CompactSqsPermissions(this Policy policy, string sqsQueueArn)
        {
            var statementsToCompact = policy.Statements
                .Where(s => s.Effect == Statement.StatementEffect.Allow)
                .Where(s => s.Actions.All(a => string.Equals(a.ActionName, SQSActionIdentifiers.SendMessage.ActionName, StringComparison.OrdinalIgnoreCase)))
                .Where(s => s.Resources.All(r => string.Equals(r.Id, sqsQueueArn, StringComparison.OrdinalIgnoreCase)))
                .Where(s => s.Principals.All(r => string.Equals(r.Id, "*", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (statementsToCompact.Count < 2)
            {
                return;
            }

            var topicArns = new HashSet<string>();
            foreach (var statement in statementsToCompact)
            {
                policy.Statements.Remove(statement);
                foreach (var topicArn in statement.Conditions.SelectMany(c => c.Values))
                {
                    topicArns.Add(topicArn);
                }
            }

            policy.AddSqsPermissions(topicArns, sqsQueueArn);
        }
    }
}