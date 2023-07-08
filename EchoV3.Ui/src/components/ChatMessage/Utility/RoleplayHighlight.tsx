// react
import React from 'react';

export const HighlightCharacters = new Map([
  [
    '((',
    {
      highlightClass: 'ooc-span',
      endCharacter: '))',
    },
  ],
  [
    '"',
    {
      highlightClass: 'quoted-span',
      endCharacter: '"',
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
      highlightClass: 'quoted-span',
      endCharacter: '’',
    },
  ],
  [
    '“',
    {
      highlightClass: 'quoted-span',
      endCharacter: '”',
    },
  ],
]);
const HighlightKeys = Array.from(HighlightCharacters.keys());

interface HighlightMatchData {
  key: string;
  index: number;
  endCharacter: string;
}

interface HighlightMatchResult {
  startIdx: number;
  endIdx: number;
  highlightClass: string;
}

const CheckHighlightStart = (text: string, idx: number) => {
  for (let j = 0; j < HighlightKeys.length; j++) {
    const key = HighlightKeys[j];
    if (idx + key.length < text.length) {
      // possible match!
      // compare substrings
      const subStr = text.substring(idx, idx + key.length);
      if (subStr === key) {
        // match!
        return {
          key,
          index: idx,
          endCharacter: HighlightCharacters.get(key)?.endCharacter as string,
        };
      }
    }
  }
  return null;
};

const CheckHighlightEnd = (
  text: string,
  idx: number,
  matchData: HighlightMatchData,
) => {
  // check length
  if (idx + matchData.endCharacter.length - 1 < text.length) {
    // check match
    if (
      text.substring(idx, idx + matchData.endCharacter.length) ===
      matchData.endCharacter
    ) {
      return true;
    }
  }
  return false;
};

const GetNodeText = (node: React.ReactNode): string => {
  if (['string', 'number'].includes(typeof node)) return node as string;
  if (node instanceof Array) return node.map(GetNodeText).join('');
  if (typeof node === 'object' && node)
    return GetNodeText(
      (node as { props: { children?: React.ReactNode } }).props.children,
    );
  throw new Error('Something went wrong!');
};

const HandleMatch = (
  key: string,
  original: string,
  results: React.ReactNode[],
  matchData: HighlightMatchResult,
) => {
  if (results.length === 0) {
    const beforeSpan = original.substring(0, matchData.startIdx);
    const bracketSpan = original.substring(matchData.startIdx, matchData.endIdx);
    const afterSpan = original.substring(matchData.endIdx, original.length);
    if (beforeSpan.length > 0) results.push(beforeSpan);
    results.push(
      <span
        key={`${key}-${results.length}`}
        className={matchData.highlightClass}
        data-testid={matchData.highlightClass}
      >
        {bracketSpan}
      </span>,
    );
    if (afterSpan.length > 0) results.push(afterSpan);
  } else {
    let startSegment;
    let startSegmentOffset;
    let segmentIndex = 0;
    const newResults: React.ReactNode[] = [];
    for (let i = 0; i < results.length; i++) {
      const currentNode = results[i];
      if (typeof currentNode === 'string') {
        // treat it normally!
        if (startSegment === undefined) {
          // a
          if (
            matchData.startIdx >= segmentIndex &&
            matchData.endIdx <= segmentIndex + currentNode.length
          ) {
            // we are entirely contained within this node!
            const beforeSpan = currentNode.substring(
              0,
              matchData.startIdx - segmentIndex,
            );
            const spanContents = currentNode.substring(
              matchData.startIdx - segmentIndex,
              matchData.endIdx - segmentIndex,
            );
            const afterSpan = currentNode.substring(
              matchData.endIdx - segmentIndex,
              currentNode.length,
            );
            if (beforeSpan.length > 0) newResults.push(beforeSpan);
            newResults.push(
              <span
                key={`${key}-${results.length}`}
                className={matchData.highlightClass}
                data-testid={matchData.highlightClass}
              >
                {spanContents}
              </span>,
            );
            if (afterSpan.length > 0) newResults.push(afterSpan);
            results.splice(i, 1, ...newResults);
            return;
          } else {
            // its split up!
            // look for segment start
            if (matchData.startIdx <= segmentIndex + currentNode.length) {
              startSegment = i;
              startSegmentOffset = matchData.startIdx - segmentIndex;
            }
          }
        } else {
          // searching for completion segment
          if (
            matchData.endIdx >= segmentIndex &&
            matchData.endIdx <= segmentIndex + currentNode.length
          ) {
            // found it!
            // slice anything between start and end
            const startNode = results[startSegment] as string;
            const beforeSpan = startNode.substring(0, startSegmentOffset);
            const beforeSpanSplit = startNode.substring(
              startSegmentOffset as number,
              startNode.length,
            );
            const spanContents = results.slice(startSegment + 1, i);
            const afterSpanSplit = currentNode.substring(
              0,
              matchData.endIdx - segmentIndex,
            );
            const afterSpan = currentNode.substring(
              matchData.endIdx - segmentIndex,
              currentNode.length,
            );

            // aa
            const spanResults: React.ReactNode[] = [];
            if (beforeSpan.length > 0) newResults.push(beforeSpan);
            if (beforeSpanSplit.length > 0) spanResults.push(beforeSpanSplit);
            if (spanContents.length > 0) spanResults.push(...spanContents);
            if (afterSpanSplit.length > 0) spanResults.push(afterSpanSplit);
            newResults.push(
              <span
                key={`${key}-${results.length}`}
                className={matchData.highlightClass}
                data-testid={matchData.highlightClass}
              >
                {spanResults}
              </span>,
            );
            if (afterSpan.length > 0) newResults.push(afterSpan);
            results.splice(startSegment, i - startSegment + 1, ...newResults);
            return;
          }
        }
        segmentIndex += currentNode.length;
      } else {
        // a
        const currentNodeText = GetNodeText(currentNode);
        // newResults.push(currentNode);
        segmentIndex += currentNodeText.length;
      }
    }
  }
};

// this function can cross nodes, but should not attempt to split them
// e.g. (( hello <span> </span> world )) -- should work
// but (( hello <span> )) world</span> -- should not detect a pair
export const RoleplayHighlight = (key: string, message: string) => {
  const results: React.ReactNode[] = [];

  if (typeof message === 'string') {
    // single iteration
    const matchStack: HighlightMatchData[] = [];
    for (let i = 0; i < message.length; i++) {
      let skip = false;
      if (matchStack.length > 0) {
        // it's possible to find a match!
        for (let stackIdx = matchStack.length - 1; stackIdx >= 0; stackIdx--) {
          const stackItem = matchStack[stackIdx];
          if (CheckHighlightEnd(message, i, stackItem)) {
            // Success!
            HandleMatch(`${key}-${i}`, message, results, {
              startIdx: stackItem.index,
              endIdx: i + stackItem.endCharacter.length,
              highlightClass: HighlightCharacters.get(stackItem.key)
                ?.highlightClass as string,
            });
            matchStack.splice(stackIdx, 1);
            skip = true;
            break;
          }
        }
      }
      if (skip) {
        continue;
      }
      const startMatch = CheckHighlightStart(message, i);
      if (startMatch !== null) {
        // match
        matchStack.push(startMatch);
        i = i + startMatch.endCharacter.length - 1;
      }
    }
  }

  // otherwise we get nothing!
  if (results.length === 0) {
    results.push(<React.Fragment key={key}>{message}</React.Fragment>);
  }

  return results;
};
