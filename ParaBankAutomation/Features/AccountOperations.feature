Feature: Account Operations
  As a ParaBank customer
  I want to view my account details and perform transfers
  So that I can manage my finances effectively

  Background:
    Given I am on the ParaBank home page
    And I am logged in as a valid user

  @smoke @regression
  Scenario: View account overview
    When I navigate to the accounts overview page
    Then I should see a list of my accounts
    And each account should display account number and balance

  @regression
  Scenario: Transfer funds between accounts
    Given I have multiple accounts with sufficient balance
    When I navigate to the transfer funds page
    And I transfer 100 dollars from account 1 to account 2
    Then I should see a transfer confirmation message
    And the transfer should be reflected in the account balances

  @regression
  Scenario: View account activity
    When I navigate to account activity for my first account
    Then I should see the account details
    And I should see a list of recent transactions
    And each transaction should display date, description, and amount

  @regression
  Scenario: Validate insufficient funds transfer
    Given I have multiple accounts
    When I navigate to the transfer funds page
    And I attempt to transfer 999999 dollars from account 1 to account 2
    Then I should see an error message indicating insufficient funds