// react
import React from 'react';

export const QuoteCharacters = new Map([
  [
    '"',
    {
      endQuote: '"',
    },
  ],
  // this method has been defeated by plural nouns ending with "s'"
  // so it's commented until I can figure out a solution
  /*[
    "'",
    {
      endQuote: "'",
    },
  ],*/
  [
    '‘',
    {
      endQuote: '’',
    },
  ],
  [
    '“',
    {
      endQuote: '”',
    },
  ],
]);

export const QuoteHighlight = (message: string) => {
  let currString = '';
  const results: React.ReactNode[] = [];
  for (let idx = 0; idx < message.length; idx++) {
    const charAtIdx = message[idx];
    const QuoteData = QuoteCharacters.get(charAtIdx);
    if (QuoteData) {
      // we are now inside of a quote!
      let quoteTerminatorIdx;
      for (let innerIdx = idx + 1; innerIdx < message.length; innerIdx++) {
        const innerChar = message[innerIdx];
        if (innerChar === QuoteData.endQuote) {
          // complete this quote!
          quoteTerminatorIdx = innerIdx;
          break;
        }
      }
      // check for terminator marker
      if (quoteTerminatorIdx !== undefined) {
        // add quoted string span to result
        if (currString.length > 0) {
          results.push(currString);
        }
        results.push(
          <span
            data-testid="quoted-span"
            key={`${message}-quote-${results.length}`}
            className="quoted-span"
          >
            {message.substring(idx, quoteTerminatorIdx + 1)}
          </span>,
        );
        // adjust idx to skip substring
        idx = quoteTerminatorIdx;
        currString = '';
        continue;
      }
    }
    currString += charAtIdx;
  }
  if (currString.length > 0) {
    results.push(currString);
  }
  return results;
};
