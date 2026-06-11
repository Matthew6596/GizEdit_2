# GizEdit

GizEdit is a level modding tool for the classic nu2 engine games (primarily focused on TCS). Being created alongside this project is a file documentation effort which you can visit here: [TT Classic File Formats](https://docs.google.com/document/d/1evg4BxJJGkiHe3INnVg00FlatldhoMZ2qTF7b_85l0c/edit?usp=sharing).

Here is the [Latest Release](https://github.com/Matthew6596/GizEdit_2/releases/latest)

<img width="1918" height="1037" alt="Screenshot of GizEdit with Negotiations_A level loaded." src="https://github.com/user-attachments/assets/188e931a-6a32-4919-9d05-c070d4d9e861" />


## Usage

GizEdit's main focus is the GIZ file, the non-ai interactable elements of a level, but the GIZ file has connections to other files as well such as the GIT file. GizEdit aims to be an editor that lets you customize the interactive elements of levels.

Check out these documents for getting started with GizEdit:

- [Quickstart](https://docs.google.com/document/d/12wxwEFJwd7b7OyYMmzvj0IsGRVtNVv3s2wXyaa8_6U8/edit?usp=sharing)
- [Getting Started](https://docs.google.com/document/d/1rMnwPIbYUbe9WKULy0LlHcTsz1CgvkWD2dvnZYNcugQ/edit?usp=sharing)

## Project Roadmap

These are the goals for GizEdit:

- [ ] Full GIZ file editing support
  - [x] TCS
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
