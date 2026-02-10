# Release 8.0

|Commit|Date|Author|Message|
|---|---|---|---|
| 8deceeb | <span style="white-space:nowrap;">2022-12-21</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (tag: 1.0.79) update github api key
| df8a4a8 | <span style="white-space:nowrap;">2024-02-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to net8.0 and use Azure.Messaging.ServiceBus instead of now deprecated Microsoft.Azure.ServiceBus
| 252070f | <span style="white-space:nowrap;">2024-02-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to vs2022 for net8
| 1e280e3 | <span style="white-space:nowrap;">2024-02-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  output singular cli exe that takes a verb/command as first parameter
| 19ca7a5 | <span style="white-space:nowrap;">2024-02-14</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  output singular cli exe that takes a verb/command as first parameter
| 27bcdec | <span style="white-space:nowrap;">2024-06-26</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  remove unused package
| c24660a | <span style="white-space:nowrap;">2024-07-05</span> | <span style="white-space:nowrap;">Erik</span> |  wire up shovel for service consumption
| 22e388c | <span style="white-space:nowrap;">2024-07-05</span> | <span style="white-space:nowrap;">Erik</span> |  generic return type
| 2b0e035 | <span style="white-space:nowrap;">2024-07-05</span> | <span style="white-space:nowrap;">Erik</span> |  wire up queue command for service
| 5069f99 | <span style="white-space:nowrap;">2024-07-05</span> | <span style="white-space:nowrap;">Erik</span> |  create and wire up peek command
| 6206a11 | <span style="white-space:nowrap;">2024-07-05</span> | <span style="white-space:nowrap;">Erik</span> |  create and wire up delete command
| 833d916 | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  return all runtime info
| b40cc1d | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  add pocos for return objects
| 7fa3eb5 | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  publish nuget
| 4c7280c | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  tweak gci
| eb5bcdb | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  output
| b3f9b44 | <span style="white-space:nowrap;">2024-07-09</span> | <span style="white-space:nowrap;">Erik</span> |  twek
| aaa42e5 | <span style="white-space:nowrap;">2024-07-10</span> | <span style="white-space:nowrap;">Erik</span> |  some clean up and error handling
| 9e7855e | <span style="white-space:nowrap;">2024-07-10</span> | <span style="white-space:nowrap;">Erik</span> |  tweak for added pckg
| f3bb453 | <span style="white-space:nowrap;">2024-07-11</span> | <span style="white-space:nowrap;">Erik</span> |  move exceptions
| 3c8292a | <span style="white-space:nowrap;">2024-07-11</span> | <span style="white-space:nowrap;">Erik</span> |  trimming limitations, use net6
| 68256bb | <span style="white-space:nowrap;">2024-07-11</span> | <span style="white-space:nowrap;">Erik</span> |  (origin/feature/clitweaks) add tests
| 77193ca | <span style="white-space:nowrap;">2024-07-12</span> | <span style="white-space:nowrap;">Erik</span> |  comment out
| be15ef7 | <span style="white-space:nowrap;">2024-07-12</span> | <span style="white-space:nowrap;">Erik</span> |  centralize message body handling, fix help
| caa186d | <span style="white-space:nowrap;">2024-07-17</span> | <span style="white-space:nowrap;">Erik</span> |  log warning
| bcc1ea6 | <span style="white-space:nowrap;">2024-08-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add support for a config file with environments defined in it
| 8f9dc36 | <span style="white-space:nowrap;">2024-08-27</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  revert to previous build.ps1
| e24a9a5 | <span style="white-space:nowrap;">2024-08-28</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix failing test
| b1bbb6f | <span style="white-space:nowrap;">2024-09-11</span> | <span style="white-space:nowrap;">Erik</span> |  add option to shovel by messageId
| 23fb7f3 | <span style="white-space:nowrap;">2024-09-19</span> | <span style="white-space:nowrap;">Erik</span> |  update queuecommand to use new ServiceBusAdministrationClient
| 144dc92 | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Erik</span> |  remove deprecated Microsoft.Azure.ServiceBus
| 60b0ce9 | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Erik</span> |  bit of info
| 20cfd75 | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #6 from cortside/feature/morecommands
| f0e7f03 | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update deploy steps
| b81b4dc | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge branch 'feature/morecommands' of github.com:cortside/amqptools into feature/morecommands
| 9b90f8a | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (origin/feature/morecommands, feature/morecommands) update deploy steps
| c64e089 | <span style="white-space:nowrap;">2024-09-20</span> | <span style="white-space:nowrap;">Erik</span> |  Merge branch 'feature/morecommands' into develop
| 9239adc | <span style="white-space:nowrap;">2024-10-01</span> | <span style="white-space:nowrap;">Erik</span> |  add protocol to env config
| 2272792 | <span style="white-space:nowrap;">2024-10-02</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #7 from cortside/feature/protocol
| bb31642 | <span style="white-space:nowrap;">2024-12-30</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add badges to README.md
| eb3d1f8 | <span style="white-space:nowrap;">2025-03-12</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Use Shouldly instead of FluentAssertions because of new licensing
| 155dcd9 | <span style="white-space:nowrap;">2025-03-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update to net8; update scripts; update deploy
| 29b6792 | <span style="white-space:nowrap;">2025-03-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update packages
| 5a096b3 | <span style="white-space:nowrap;">2025-03-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  ignore amqptools.json
| 05bc6e5 | <span style="white-space:nowrap;">2025-03-13</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  updates for build
| 113caf9 | <span style="white-space:nowrap;">2025-03-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add back ignored publish command and add dotnet tool metadata to project
| 02c1e7b | <span style="white-space:nowrap;">2025-03-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  attempt to get tool package registered
| aea6ad8 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix async handling
| fba2712 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  fix async handling; make output parsable json
| 4018949 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add README.md to nuget package
| ff8a839 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  set tool name with dotnet prefix
| e661f78 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  updates to help help to show up as helpful
| 74ae5e1 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update build
| 1ec30d7 | <span style="white-space:nowrap;">2025-03-25</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (async) update nuget packages
| d2c3802 | <span style="white-space:nowrap;">2025-05-22</span> | <span style="white-space:nowrap;">=</span> |  (origin/async) ignore dir; local install script
| 7ff2988 | <span style="white-space:nowrap;">2025-11-20</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  Merge pull request #8 from cortside/async
| 3cdec5f | <span style="white-space:nowrap;">2026-02-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget packages and make sure tests pass
| 32b1b0d | <span style="white-space:nowrap;">2026-02-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 14f0018 | <span style="white-space:nowrap;">2026-02-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  update nuget api key
| 549c02c | <span style="white-space:nowrap;">2026-02-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  add empty changelog to start
| 8cd587b | <span style="white-space:nowrap;">2026-02-10</span> | <span style="white-space:nowrap;">Cort Schaefer</span> |  (HEAD -> release/8.0, origin/develop, develop) add empty changelog to start
****

# Release 1.0
* see github and nuget.org for versions and information
|Commit|Date|Author|Message|
|---|---|---|---|
****
