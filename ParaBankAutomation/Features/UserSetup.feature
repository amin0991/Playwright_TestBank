@Setup @Ignore
Feature: Test User Setup
  As a test automation framework
  I need to create test users automatically
  So that tests can run without manual setup

  @SetupUser
  Scenario: Create john user account
    Given I am on the ParaBank registration page
    When I register a new user with username "john"
    Then the user should be created successfully
    And the user should have at least one account
    When I open a second account for the user
    Then the user should have 2 accounts

  @SetupUser
  Scenario: Create highbrow90 user account
    Given I am on the ParaBank registration page
    When I register a new user with username "highbrow90"
    Then the user should be created successfully
    And the user should have at least one account
    When I open a second account for the user
    Then the user should have 2 accounts