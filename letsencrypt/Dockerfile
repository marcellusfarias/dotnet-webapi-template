FROM alpine:latest

# Update the package index and upgrade installed packages
RUN apk --no-cache update && apk --no-cache upgrade
RUN apk add certbot

# Create volumes
VOLUME ["/letsencrypt"]
VOLUME ["/etc/letsencrypt"]

# Do nothing, but keeps executing
CMD ["tail", "-f", "/dev/null"]