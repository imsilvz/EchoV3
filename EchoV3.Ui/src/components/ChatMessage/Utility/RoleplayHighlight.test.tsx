import React from 'react';
import { assert, describe, expect, it } from 'vitest';
import { render, screen } from '@testing-library/react';
import { HighlightCharacters, RoleplayHighlight } from './RoleplayHighlight';

describe('Bracket Highlighting', () => {
  it('no highlight test', () => {
    const testMessage = 'This is a test message';
    render(
      <div data-testid="highlight-container">
        {RoleplayHighlight('test-key', testMessage)}
      </div>,
    );
    const oocTest = screen.queryByTestId('ooc-span');
    const quoteTest = screen.queryByTestId('quoted-span');
    const containerTest = screen.queryByTestId('highlight-container');
    expect(oocTest).not.toBeInTheDocument();
    expect(quoteTest).not.toBeInTheDocument();
    expect(containerTest).toBeInTheDocument();
    expect(containerTest).toHaveTextContent(testMessage);
  });

  it('single ooc test', () => {
    const testMessage = 'This is a ((test)) message';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpan = screen.queryByTestId('ooc-span');
    expect(testSpan).toBeInTheDocument();
    expect(testSpan).toHaveTextContent('test');
  });

  it('single quotes test', () => {
    const testMessage = 'This is a "test" message';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpan = screen.queryByTestId('quoted-span');
    expect(testSpan).toBeInTheDocument();
    expect(testSpan).toHaveTextContent('test');
  });

  it('multi ooc test', () => {
    const testMessage = 'This ((is)) a ((test))(( message))';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpans = screen.getAllByTestId('ooc-span');
    assert(testSpans.length === 3);
    expect(testSpans[0]).toHaveTextContent('is');
    expect(testSpans[1]).toHaveTextContent('test');
    expect(testSpans[2]).toHaveTextContent('message');
  });

  it('multi quotes test', () => {
    const testMessage = 'This "is" "a test" "message"';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 3);
    expect(testSpans[0]).toHaveTextContent('is');
    expect(testSpans[1]).toHaveTextContent('a test');
    expect(testSpans[2]).toHaveTextContent('message');
  });

  it('nested brackets test', () => {
    const testMessage = 'This ((is ((a)) test)) message';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpans = screen.getAllByTestId('ooc-span');
    assert(testSpans.length === 2);
    expect(testSpans[0]).toHaveTextContent('((is ((a)) test))');
    expect(testSpans[1]).toHaveTextContent('((a))');
  });

  it('nested quotes test', () => {
    const testMessage = 'This "is ‘a’ test" message';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 2);
    expect(testSpans[0]).toHaveTextContent('"is ‘a’ test"');
    expect(testSpans[1]).toHaveTextContent('‘a’');
  });

  it('nested brackets inside quote', () => {
    const testMessage = 'This ((is "a" test)) message';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const quoteSpan = screen.queryByTestId('quoted-span');
    const bracketSpan = screen.queryByTestId('ooc-span');
    expect(quoteSpan).toBeInTheDocument();
    expect(bracketSpan).toBeInTheDocument();
  });

  it('test incomplete', () => {
    const testMessage = '"Hello!" ((what\'s happening? "aaa"';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const bracketSpan = screen.queryByTestId('ooc-span');
    const quoteSpans = screen.getAllByTestId('quoted-span');
    expect(bracketSpan).not.toBeInTheDocument();
    assert(quoteSpans.length === 2);
  });

  it('test quote variants', () => {
    const stringList: string[] = [];
    const quoteKeys = Array.from(HighlightCharacters.keys());
    quoteKeys.forEach((key) => {
      const endChar = HighlightCharacters.get(key)?.endCharacter as string;
      stringList.push(`${key}message${endChar}`);
    });
    const testMessage = stringList.join(' | ');
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const quoteSpans = screen.getAllByTestId('quoted-span');
    const oocSpans = screen.getAllByTestId('ooc-span');
    assert(quoteSpans.length + oocSpans.length === quoteKeys.length);
  });

  it('test mismatched quotes', () => {
    const testMessage = 'This is a test "message\' hello world';
    render(<>{RoleplayHighlight('test-key', testMessage)}</>);
    const testSpan = screen.queryByTestId('quoted-span');
    expect(testSpan).not.toBeInTheDocument();
  });
});
