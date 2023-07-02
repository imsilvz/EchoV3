# EchoV3
[![Build][build-badge]][build-url]
[![Test][test-badge]][test-url]
[![Release][release-badge]][release-url]

[build-url]: https://github.com/ff14wed/deucalion/actions/workflows/build.yml
[build-badge]: https://github.com/imsilvz/EchoV3/actions/workflows/build.yml/badge.svg
[test-url]: https://github.com/ff14wed/deucalion/actions/workflows/test.yml
[test-badge]: https://github.com/imsilvz/EchoV3/actions/workflows/test.yml/badge.svg
[release-url]: https://github.com/imsilvz/EchoV3/releases/latest
[release-badge]: https://img.shields.io/github/v/release/imsilvz/EchoV3

An external message log meant to improve readability for FFXIV's chat. It utilizes
[deucalion](https://github.com/ff14wed/deucalion) to read and decode application packets, 
and then uses that data to parse to enhance the chat experience.

## Features
  - ***Echo*** acts as an external chatbox. Resize and position it anywhere on your screen, on a second monitor, or beside your existing chatbox!
  - ***Filter Channels*** at will through a simple context menu. You can show or hide a channel with a single click.
  - ***Listener Mode*** can be toggled so that you will only see the messages sent by your in-game target. No longer will a flood of messages interrupt your one-on-one conversations!
  - ***Name Highlighting*** colors playernames based on one of three coloration strategies: custom (user-specified), random, or job-based.
  - ***Roleplay Highlighting*** helps enhance readability by highlighting quotations in roleplay-flagged channels.
