@Pending
# Tests may be irrelevant now with changes to group permissions being opened up.
Feature: groupPrivate
    User journeys covering functionality and permissions when working with a private group

Background: 
    Given I have navigated to '/'
    Given I have logged in as a 'admin'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed


Scenario: FNHS37 - Members Table Validation
    When I click the 'Automation Private Group' link
    Then the 'Automation Private Group' header is displayed
    When I click the 'Members' tab
    And the 'Pending members' table is displayed
    | Name       | Email                | Request date | Accept | Reject |
    | Auto User3 | autoUser3@test.co.uk | [PrettyDate] | Accept | Reject |
    And the 'Group Members' table is displayed
    | Name       | Role             | Date joined  | Last logged in | Edit |
    | auto Admin | Admin            | [PrettyDate] | [PrettyDate]   | Edit |
    | Auto User2 | Standard Members | [PrettyDate] | [PrettyDate]   | Edit |
    

Scenario: FNHS38 - Pending Approval: Group Home validation
    Given I log off and log in as an 'user'
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
    When I click the 'AutoPendingGroup' link
    Then the 'AutoPendingGroup' header is displayed
    When the 'Pending Approval' link is displayed
    Then the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed


Scenario: FNHS39 - Pending Approval: View group forum
    Given I log off and log in as an 'user'
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
    When I click the 'AutoPendingGroup' link
    Then the 'AutoPendingGroup' header is displayed
    When I click the 'Forum' tab
    And the 'You must be a member of this group to access this content' textual value is displayed


Scenario: FNHS40 - Pending Approval: View group members
    Given I log off and log in as an 'user'
    When I click the 'Discover new groups' tab
    Then the 'Discover New Groups' header is displayed
    When I click the 'AutoPendingGroup' link
    Then the 'AutoPendingGroup' header is displayed
    When I click the 'Members' tab
    And the 'You must be a member of this group to access this content' textual value is displayed
    
@Core 
Scenario: FNHS41 - Join a private group 
    Given I log off and log in as an 'user'    
    When I click the 'Discover new groups' tab
    And I click the 'Automation Private Group' link
    Then the 'Automation Private Group' header is displayed
    And the 'Join Group' link is displayed
    When I click the 'Join Group' link
    Then the 'Pending Approval' textual value is displayed

@Core 
Scenario: FNHS42 - Add member awaiting approval
    When I click the 'Automation Private Group' link
    Then the 'Automation Private Group' header is displayed
    When I click the 'Members' tab
    Then the 'Auto User' row is displayed on the 'Pending Members' table
    When I select 'Add new member' from the group pages accordion
    Then the 'Invite member' header is displayed
    When I enter 'autoUser@test.co.uk' into the 'New member email address' field
    And I click the 'Add new member' option
    Then the 'Automation Private Group' header is displayed
    When I click the 'Members' tab
    Then the 'Auto User' row is displayed on the 'Group Members' table