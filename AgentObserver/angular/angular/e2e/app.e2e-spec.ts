import { AgentObserverTemplatePage } from './app.po';

describe('AgentObserver App', function() {
  let page: AgentObserverTemplatePage;

  beforeEach(() => {
    page = new AgentObserverTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
