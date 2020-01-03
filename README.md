# drone-hugo

A plugin for Drone to work with Hugo Static Site Generator.

## Usage

The options are very similar to the already existing drone hugo plugin.

```yaml
- name: deploy
  image: hypervtechnics/drone-hugo
  settings:
    validate: false                 # Whether to validate using Hugo CLI before the build
    hugo_version: 0.62.0            # The Hugo version. If it is empty the latest one is downloaded.
    url: https://example.com/blog   # The base URL
    theme: some-theme               # The theme to use
    builddrafts: false              # Whether to build drafts entries
    buildexpired: false             # Whether to build expired entries
    buildfuture: false              # Whether to build future entries
    minify: false                   # Minify all assets
    config: ./config.yml            # The path to the configuration file
    cache: [...]                    # The cache directory
    content: [...]                  # The content directory
    layout: [...]                   # The layout directory
    source: [...]                   # The source directory
    output: ./public                # The output directory
```

## Docker

I don't have the time/need for a tag per hugo version. Meaning you can use something like `.../drone-hugo:0.62.0`. 

The application supports a download-only mode you can invoke during `docker build` in the `Dockerfile` using something like `RUN dotnet /app/Drone.Hugo.dll download 0.62.0`. The binary is then saved to `/app/assets/hugo`. If there is a binary at this location it is always used.
