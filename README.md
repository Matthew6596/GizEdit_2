# GizEdit

GizEdit is a level modding tool for the classic nu2 engine games (primarily focused on TCS). Being created alongside this project is a file documentation effort which you can visit here: [TT Classic File Formats](https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?usp=sharing).

Here is the [Latest Release](https://github.com/Matthew6596/GizEdit_2/releases/latest)

## Usage

GizEdit's main focus is the GIZ file, the non-ai interactable elements of a level, but the GIZ file has connections to other files as well such as the GIT file. GizEdit aims to be an editor that empowers you to customize the interactive elements of levels.

## Project Roadmap

This project has ambitious goals and some may not be reached for a long time.

- [ ] Full GIZ file editing support
  - [ ] TCS
  - [ ] LIJ1
  - [ ] LB1
- [ ] GSC file readonly support
  - [ ] TCS
  - [ ] LIJ1
  - [ ] LB1

# Contribution

If you'd like to contribute to this project, there are many ways:
- File Documentation (helpful for others' projects too)
  - Researching file formatting/versions
  - Researching unknown properties
  - Researching unknown aspects/quirks of known properties
- GizEdit Development
  - Adding new property subclasses
  - Adding new editor gizmos
  - Implementing data loaders
  - Aesthetic or QOL changes
  - Testing and bug reporting

 ## Development Considerations

- Uses Unity 6000.2.8f1 and is meant to be multiplatform for Windows, Mac, and Linux.
- Designed to have UI of a fixed physical size.
- The project MUST NOT contain any copyrighted assets (assets from TT's games must be loaded from the user's files at runtime).
- User experience is a major consideration in design choices
