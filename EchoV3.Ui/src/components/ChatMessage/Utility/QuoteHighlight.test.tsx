import React from 'react';
import { assert, describe, expect, it } from 'vitest';
import { render, screen } from '@testing-library/react';
import { QuoteCharacters, QuoteHighlight } from './QuoteHighlight';

describe('Quote Highlighting', () => {
  it('no quote highlight', () => {
    const testMessage = 'This is a test message hello world';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpan = screen.queryByTestId('quoted-span');
    expect(testSpan).not.toBeInTheDocument();
  });
  it('single quote highlight', () => {
    const testMessage = 'This is a test "message" hello world';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpan = screen.getAllByTestId('quoted-span');
    assert(testSpan.length === 1);
    expect(testSpan[0]).toHaveTextContent('"message"');
  });
  it('multi quote highlight', () => {
    const testMessage = 'This "is a"" test ""message" hello world';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 3);
  });
  it('starting quote highlight', () => {
    const testMessage = '"This" "is a"" test ""message" hello world';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 4);
  });
  it('ending quote highlight', () => {
    const testMessage = 'This "is a"" test ""message" hello "world"';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 4);
  });
  it('incomplete quote highlight', () => {
    const testMessage = 'This "is a" test ""message" hello "world"';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === 3);
  });
  it('test quote variants', () => {
    const stringList: string[] = [];
    const quoteKeys = Array.from(QuoteCharacters.keys());
    quoteKeys.forEach((key) => {
      const endChar = QuoteCharacters.get(key)?.endQuote as string;
      stringList.push(`${key}message${endChar}`);
    });
    const testMessage = stringList.join(' | ');
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpans = screen.getAllByTestId('quoted-span');
    assert(testSpans.length === quoteKeys.length);
  });
  it('test mismatched quotes', () => {
    const testMessage = 'This is a test "message\' hello world';
    render(<>{QuoteHighlight(testMessage)}</>);
    const testSpan = screen.queryByTestId('quoted-span');
    expect(testSpan).not.toBeInTheDocument();
  });
});
