﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.ServiceBus.Models;
using Microsoft.Azure.Management.ServiceBus.Models;
using System.Management.Automation;
using System.Collections.Generic;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.Queue
{
    /// <summary>
    /// 'New-AzureRmServiceBusQueueAuthorizationRule' Cmdlet creates a new AuthorizationRule
    /// </summary>
    [Cmdlet(VerbsCommon.New, ServiceBusQueueAuthorizationRuleVerb, SupportsShouldProcess = true), OutputType(typeof(SharedAccessAuthorizationRuleAttributes))]
    public class NewAzureRmServiceBusQueueAuthorizationRule : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "The name of the resource group")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroup { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            HelpMessage = "Namespace Name.")]
        [ValidateNotNullOrEmpty]
        public string NamespaceName { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2,
            HelpMessage = "Queue Name.")]
        [ValidateNotNullOrEmpty]
        public string QueueName { get; set; }

        [Parameter(Mandatory = true,
           ValueFromPipelineByPropertyName = true,
           Position = 2,
           HelpMessage = "AuthorizationRule Name.")]
        [ValidateNotNullOrEmpty]
        public string AuthorizationRuleName { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 3,
            HelpMessage = "Rights, e.g.  @(\"Listen\",\"Send\",\"Manage\")")]
        [ValidateNotNullOrEmpty]
        public string[] Rights { get; set; }

        public override void ExecuteCmdlet()
        {
            SharedAccessAuthorizationRuleAttributes sasRule = new SharedAccessAuthorizationRuleAttributes();

            NamespaceAttributes getNameSpace = Client.GetNamespace(ResourceGroup, NamespaceName);

            IList<Management.ServiceBus.Models.AccessRights?> newListAry = new List<Management.ServiceBus.Models.AccessRights?>();

            foreach (string rights in Rights)
            {
                newListAry.Add(ParseAccessRights(rights));
            }

            sasRule.Name = AuthorizationRuleName;
            sasRule.Rights = newListAry;
            sasRule.Location = getNameSpace.Location;

            // Create a new ServiceBus namespace authorizationRule
            if (ShouldProcess(target: AuthorizationRuleName, action: string.Format("Create new AuthorizationRule:{0} for Queue:{1} of NameSpace:{2}", AuthorizationRuleName, QueueName, NamespaceName)))
            {
                WriteObject(Client.CreateOrUpdateServiceBusQueueAuthorizationRules(ResourceGroup, NamespaceName, QueueName, sasRule.Name, sasRule));
            }
        }
    }
}
