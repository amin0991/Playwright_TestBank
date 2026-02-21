Feature: New Account Creation
  As a ParaBank customer
  I want to open new accounts
  So that I can manage different types of savings and checking accounts

  Background:
  Given I am on the ParaBank home page
  And I am logged in as a valid user

  @smoke @regression
  Scenario: Open a new savings account
    When I navigate to the open new account page
    And I select "SAVINGS" as account type
    And I select an existing account to transfer initial funds from
    And I click the open new account button
    Then I should see a confirmation message for the new account
    And the new account should appear in my accounts overview

  @regression
  Scenario: Open a new checking account
    When I navigate to the open new account page
    And I select "CHECKING" as account type
    And I select an existing account to transfer initial funds from
    And I click the open new account button
    Then I should see a confirmation message for the new account
    And the new account number should be displayed